using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obganism
{
	class ParsingContext
	{
		public string Source;
		int I;

		bool LetterIs(char @char) => LetterIs(I, @char);
		bool LetterIs(string oneOf) => LetterIs(I, oneOf);
		
		bool LetterIs(int i, char @char) => i < Source.Length && Source[i] == @char;
		bool LetterIs(int i, string oneOf) => i < Source.Length && oneOf.Contains(Source[i]);

		const string Digits = "9238461570"; // Ordered by usage frequency in PI.
		const string Letters = "etaoinsrhdlucmfywgpbvkxqjzETAOINSRHDLUCMFYWGPBVKXQJZ"; // Ordered by usage frequency in English.
		const string Spaces = " \t";
		const string LineBreaks = "\n\r";

		const string ValidCharactersInWord = Letters;
		const string ValidCharactersAfterWord = Spaces + LineBreaks + ":-(){},";
		const string Formatting = Spaces + LineBreaks;

		public IReadOnlyList<ObganismObject> ReadObjects()
		{
			var objects = ReadListOfItems(
				CanReadObject,
				DoReadObject,
				TryReadObject
			);

			if (I < Source.Length)
				throw new ObganismException(I, "An object list must end with the end of the input.");

			return objects;
		}

		delegate bool CanReadItemDelegate();
		delegate T DoReadItemDelegate<T>();
		delegate bool TryReadItemDelegate<T>(out T @out);

		IReadOnlyList<T> ReadListOfItems<T>(CanReadItemDelegate canReadItem, DoReadItemDelegate<T> doReadItem, TryReadItemDelegate<T> tryReadItem, string atLeastOne = null)
		{
			while (LetterIs(Formatting))
				++I;

			if (tryReadItem(out var item))
			{
				var items = new List<T> { item };

				ANCHOR_1:

				if (LetterIs(Spaces))
				{
					++I;
					goto ANCHOR_1;
				}

				else if (LetterIs(LineBreaks))
				{
					++I;

					while (LetterIs(Formatting))
						++I;

					if (LetterIs(','))
						goto ANCHOR_1;

					else if (tryReadItem(out item))
					{
						items.Add(item);
						goto ANCHOR_1;
					}

					else return items;
				}

				else if (LetterIs(','))
				{
					++I;

					while (LetterIs(Formatting))
						++I;

					items.Add(doReadItem());
					goto ANCHOR_1;
				}

				else return items;
			}

			else if (atLeastOne != null)
				throw new ObganismException(I, atLeastOne);

			else return new List<T>(0);
		}

		bool CanReadObject()
		{
			return I < Source.Length;
		}

		ObganismObject DoReadObject()
		{
			var type = DoReadType();
			var properties = new List<ObganismProperty>(0) as IReadOnlyList<ObganismProperty>;
			var save = I;

			while (LetterIs(Formatting))
				++I;

			if (LetterIs('{'))
			{
				++I;
				properties = ReadListOfItems(CanReadProperty, DoReadProperty, TryReadProperty);

				if (LetterIs('}')) ++I;
				else throw new ObganismException(I, "A property list must end with a brace.");
			}
			
			else I = save;

			var modifiers = ReadModifiers();

			return new ObganismObject { Type = type, Properties = properties, Modifiers = modifiers };
		}

		bool TryReadObject(out ObganismObject @out)
		{
			@out = default;

			if (CanReadObject())
			{
				@out = DoReadObject();
				return true;
			}
			
			else return false;
		}

		bool CanReadProperty()
		{
			return CanReadName();
		}

		ObganismProperty DoReadProperty()
		{
			var name = DoReadName();

			while (LetterIs(Formatting))
				++I;

			if (LetterIs(':')) ++I;
			else throw new ObganismException(I, "The name of a property must be followed by a colon.");

			while (LetterIs(Formatting))
				++I;

			var type = DoReadType();
			var modifiers = ReadModifiers();

			return new ObganismProperty { Name = name, Type = type, Modifiers = modifiers };
		}

		IReadOnlyList<ObganismModifier> ReadModifiers()
		{
			var save = I;

			while (LetterIs(Formatting))
				++I;

			if (LetterIs('-'))
			{
				++I;

				if (LetterIs('-'))
				{
					++I;

					while (LetterIs(Formatting))
						++I;

					if (LetterIs('('))
					{
						++I;
						var modifiers = ReadListOfItems(CanReadModifier, DoReadModifier, TryReadModifier, "A modifier list must contain at least 1 item.");

						if (LetterIs(')')) ++I;
						else throw new ObganismException(I, "A modifier list must end with a parenthesis.");

						return modifiers;
					}

					else return new List<ObganismModifier> { DoReadModifier() };
				}

				else throw new ObganismException(I, "Modifiers must be introduced by 2 dashes.");
			}

			else I = save;

			return new List<ObganismModifier>(0);
		}

		bool TryReadProperty(out ObganismProperty @out)
		{
			@out = default;

			if (CanReadProperty())
			{
				@out = DoReadProperty();
				return true;
			}

			else return false;
		}
		
		bool CanReadModifier()
		{
			return CanReadName();
		}

		ObganismModifier DoReadModifier()
		{
			var name = DoReadName();
			var parameters = new List<ObganismModifierParameter>(0) as IReadOnlyList<ObganismModifierParameter>;
			var save = I;

			while (LetterIs(Formatting))
				++I;

			if (LetterIs('('))
			{
				++I;
				parameters = ReadListOfItems(CanReadModifierParameter, DoReadModifierParameter, TryReadModifierParameter, "A modifier parameter list must contain at least 1 item.");

				if (LetterIs(')')) ++I;
				else throw new ObganismException(I, "A list of modifier parameters must end with a parenthesis.");
			}

			else I = save;

			return new ObganismModifier { Name = name, Parameters = parameters };
		}

		bool TryReadModifier(out ObganismModifier @out)
		{
			@out = default;

			if (CanReadModifier())
			{
				@out = DoReadModifier();
				return true;
			}

			else return false;
		}
		
		bool CanReadModifierParameter()
		{
			return LetterIs(Digits)
			    || LetterIs('"')
			    || CanReadName()
			    || LetterIs(':');
		}

		ObganismModifierParameter DoReadModifierParameter()
		{
			if (LetterIs(Digits))
			{
				var number = new StringBuilder();

				while (LetterIs(Digits))
				{
					number.Append(Source[I]);
					++I;
				}

				if (LetterIs('.'))
				{
					number.Append('.');
					++I;

					while (LetterIs(Digits))
					{
						number.Append(Source[I]);
						++I;
					}

					return new ObganismModifierParameter.Real { Value = float.Parse(number.ToString()) };
				}

				else return new ObganismModifierParameter.Integer { Value = int.Parse(number.ToString()) };
			}

			else if (LetterIs('"'))
			{
				++I;

				var @string = new StringBuilder();

				ANCHOR_1:

				while (!LetterIs('"') && I < Source.Length)
				{
					@string.Append(Source[I]);
					++I;
				}

				if (LetterIs('"'))
				{
					++I;

					if (LetterIs('"'))
					{
						@string.Append(Source[I]);
						++I;
						goto ANCHOR_1;
					}

					else return new ObganismModifierParameter.String { Value = @string.ToString() };
				}

				else throw new ObganismException(I, "A string must end with a double quote mark.");
			}

			else if (TryReadName(out var name))
				return new ObganismModifierParameter.Name { Value = name };

			else if (LetterIs(':'))
			{
				++I;

				while (LetterIs(Formatting))
					++I;

				return new ObganismModifierParameter.Type { Value = DoReadType() };
			}

			else throw new ObganismException(I, "This isn't a modifier parameter...");
		}

		bool TryReadModifierParameter(out ObganismModifierParameter @out)
		{
			@out = default;

			if (CanReadModifierParameter())
			{
				@out = DoReadModifierParameter();
				return true;
			}

			else return false;
		}

		bool CanReadType()
		{
			return CanReadTypeName();
		}

		ObganismType DoReadType()
		{
			var name = DoReadTypeName();
			var generics = new List<ObganismType>(0) as IReadOnlyList<ObganismType>;
			var save = I;

			while (LetterIs(Formatting))
				++I;

			if (TryReadOfKeyword())
			{
				while (LetterIs(Formatting))
					++I;

				if (LetterIs('('))
				{
					++I;
					generics = ReadListOfItems(CanReadType, DoReadType, TryReadType, "A generic list must contain at least 1 type.");

					if (LetterIs(')')) ++I;
					else throw new ObganismException(I, "A generic list must end with a parenthesis.");
				}

				else generics = new List<ObganismType>(1) { DoReadType() };
			}

			else I = save;

			return new ObganismType { Name = name, Generics = generics };
		}

		bool TryReadType(out ObganismType @out)
		{
			@out = default;

			if (CanReadType())
			{
				@out = DoReadType();
				return true;
			}

			else return false;
		}

		bool CanReadTypeName()
		{
			return CanReadEscapedOfKeyword()
			    || CanReadWord();
		}

		string DoReadTypeName()
		{
			if (CanReadOfKeyword())
				throw new ObganismException(I, "The name of a type must not start with the 'of' keyword.");

			var name = new StringBuilder();

			if (TryReadEscapedOfKeyword())
				name.Append("of");

			else
				name.Append(DoReadWord());

			ANCHOR_1:

			while (LetterIs(Spaces))
				++I;

			if (CanReadOfKeyword())
				return name.ToString();

			else if (TryReadEscapedOfKeyword())
			{
				name.Append(' ').Append("of");
				goto ANCHOR_1;
			}
			
			else if (TryReadWord(out var word))
			{
				name.Append(' ').Append(word);
				goto ANCHOR_1;
			}

			return name.ToString();
		}

		bool TryReadTypeName(out string @out)
		{
			@out = default;

			if (CanReadTypeName())
			{
				@out = DoReadTypeName();
				return true;
			}

			else return false;
		}

		bool CanReadName()
		{
			return CanReadWord();
		}

		string DoReadName()
		{
			var word = DoReadWord();
			var name = new StringBuilder(word);

			ANCHOR_1:

			while (LetterIs(Spaces))
				++I;
			
			if (TryReadWord(out word))
			{
				name.Append(' ').Append(word);
				goto ANCHOR_1;
			}

			return name.ToString();
		}

		bool TryReadName(out string @out)
		{
			@out = default;

			if (CanReadName())
			{
				@out = DoReadName();
				return true;
			}

			else return false;
		}

		bool CanReadWord()
		{
			return LetterIs(ValidCharactersInWord);
		}

		string DoReadWord()
		{
			if (LetterIs(ValidCharactersInWord))
			{
				var word = new StringBuilder();

				while (LetterIs(ValidCharactersInWord))
				{
					word.Append(Source[I]);
					++I;
				}

				if (LetterIs(ValidCharactersAfterWord) || Source.Length <= I)
					return word.ToString();
			}

			throw new ObganismException(I, "A word must contain only letters.");
		}

		bool TryReadWord(out string @out)
		{
			@out = default;

			if (CanReadWord())
			{
				@out = DoReadWord();
				return true;
			}

			else return false;
		}

		bool CanReadEscapedOfKeyword()
		{
			return LetterIs('\\');
		}

		void DoReadEscapedOfKeyword()
		{
			if (
				    LetterIs("\\")
				&&  LetterIs(I + 1, "oO")
				&&  LetterIs(I + 2, "fF")
				&& !LetterIs(I + 3, ValidCharactersInWord)
			)
			{
				++I;
				++I;
				++I;
			}

			else throw new ObganismException(I, "The backslash must only escape the 'of' keyword.");
		}

		bool TryReadEscapedOfKeyword()
		{
			if (CanReadEscapedOfKeyword())
			{
				DoReadEscapedOfKeyword();
				return true;
			}

			else return false;
		}

		bool CanReadOfKeyword()
		{
			return  LetterIs("oO")
			    &&  LetterIs(I + 1, "fF")
			    && !LetterIs(I + 2, ValidCharactersInWord);
		}

		void DoReadOfKeyword()
		{
			if (CanReadOfKeyword())
			{
				++I;
				++I;
			}

			else throw new ObganismException(I, "This isn't the 'of' keyword...");
		}

		bool TryReadOfKeyword()
		{
			if (CanReadOfKeyword())
			{
				++I;
				++I;
				return true;
			}

			else return false;
		}
	}
}
