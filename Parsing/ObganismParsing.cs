using System;
using System.Collections.Generic;
using System.Text;

using ObganismObject = Obganism.Definitions.Obgan; // TODO: Just rename the originals...
using ObganismType = Obganism.Definitions.Type;
using ObganismProperty = Obganism.Definitions.Property;

namespace Obganism.Parsing
{
	public static class ObganismParsing
	{
		/// <summary>
		///   Converts Obganism source code into CLR Obganism AST.
		/// </summary>
		///
		/// <param name="input">
		///   Some Obganism source code as per <see href="https://github.com/Odepax/obganism-lang/wiki"/>.
		/// </param>
		///
		/// <seealso cref="ObganismObject" />
		///
		/// <exception cref="ObganismParsingException">
		///   Thrown when the <paramref name="input"/> isn't valid Obganism,
		///   or when a bug throws an unexcpected exception, in which case <see cref="Exception.InnerException"/> will be set.
		/// </exception>
		public static IReadOnlyList<ObganismObject> ConvertFromObganism(string input)
		{
			ParsingContext context = new ParsingContext(input);

			try
			{
				ReadFormatting(context);
				ReadAny(context, TestType, ReadObject, out List<ObganismObject> objects);
				ReadFormatting(context);

				return objects;
			}
			catch (ObganismParsingException) // TODO: Replace with a when (innerException !is ObganismParsingException) clause?
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ObganismParsingException(context, innerException);
			}
		}
		/*
		private static void ReadObganism(ParsingContext context, out List<ObganismObject> @out) =>
			ReadZeroOneOrMore(
				context: context,
				skipListStart: context => {},
				readItem: ReadObgan,
				testListEnd: context => context.Source.Length <= context.Position,
				skipListEnd: context => {},
				@out: out @out
			);

		private static void ReadObgan(ParsingContext context, out ObganismObject @out)
		{
			ReadType(context, out ObganismType type);
			Checkpoint(context);
			SkipFormatting(context);
			
			if (TestOpenBrace(context))
			{
				Confirm(context);
				ReadProperties(context, out List<ObganismProperty> properties);

				@out = new ObganismObject(type, properties);
			}
			else
			{
				Restore(context);

				@out = new ObganismObject(type);
			}
		}

		private static void ReadType(ParsingContext context, out ObganismType @out)
		{
			ReadTypeName(context, out string name);

			if (TrySkip(context, SkipFormatting, TestOf, SkipOf))
			{
				SkipFormatting(context);
				ReadGenerics(context, out List<ObganismType> generics);

				@out = new ObganismType(name, generics);
			}
			else
			{
				@out = new ObganismType(name);
			}
		}

		private static void ReadTypeName(ParsingContext context, out string @out)
		{
			if (!(TestLetter(context) || TestEscapedOf(context)))
				throw new ObganismParsingException(context, "some type name");

			if (TestOf(context))
				throw new ObganismParsingException(context, "some type name", "an <<of>> keyword");

			StringBuilder words = new StringBuilder();

			ReadTypeWord();

			while (true)
			{
				SkipSpaces(context);
				AssertValidCharacter(context, "some type name");

				if (!(TestLetter(context) || TestEscapedOf(context)) || TestOf(context))
					break;
			
				words.Append(' ');

				ReadTypeWord();
			}

			@out = words.ToString();

			void ReadTypeWord()
			{
				if (context.Source[context.Position] == '\\') // Escaped 'of'.
				{
					context.Position += 3;

					words.Append(context.Source.Substring(context.Position - 2, 2));
				}
				else
				{
					int startPosition = context.Position;

					SkipWhile(context, Letters.Contains);

					words.Append(context.Source[startPosition..context.Position]);
				}
			}
		}

		private static void ReadGenerics(ParsingContext context, out List<ObganismType> @out)
		{
			if (TestOpenParenthesis(context))
			{
				ReadOneOrMore(
					context: context,
					skipListStart: SkipOpenParenthesis,
					readItem: ReadType,
					testListEnd: TestCloseParenthesis,
					skipListEnd: SkipCloseParenthesis,
					@out: out @out
				);
			}
			else
			{
				ReadType(context, out ObganismType generic);

				@out = new List<ObganismType> { generic };
			}
		}

		private static void ReadProperties(ParsingContext context, out List<ObganismProperty> @out) =>
			ReadOneOrMore(
				context: context,
				skipListStart: SkipOpenBrace,
				readItem: ReadProperty,
				testListEnd: TestCloseBrace,
				skipListEnd: SkipCloseBrace,
				@out: out @out
			);

		private static void ReadProperty(ParsingContext context, out ObganismProperty @out)
		{
			ReadPropertyName(context, out string name);
			SkipFormatting(context);

			if (TestColon(context))
				SkipColon(context);
			else
				throw new ObganismParsingException(context, $"a colon introducing the <<{ name }>> property's type");
			
			SkipFormatting(context);
			ReadType(context, out ObganismType type);

			@out = new ObganismProperty(name, type);
		}

		private static void ReadPropertyName(ParsingContext context, out string @out)
		{
			if (!TestLetter(context))
				throw new ObganismParsingException(context, "some property name");

			StringBuilder words = new StringBuilder();

			ReadWord();

			while (true)
			{
				SkipSpaces(context);

				AssertValidCharacter(context, "some property name");

				if (!TestLetter(context))
					break;

				words.Append(' ');

				ReadWord();
			}

			@out = words.ToString();

			void ReadWord()
			{
				int startPosition = context.Position;

				SkipWhile(context, Letters.Contains);

				words.Append(context.Source[startPosition..context.Position]);
			}
		}
		*/
		// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
		// OLD SHIT

		// Spaces and Breaks
		// ----------------------------------------------------------------

		private static void ReadSpacing(ParsingContext context)
		{
			while (Test(context, Spaces))
				++context.Position;
		}

		private static void ReadFormatting(ParsingContext context)
		{
			while (Test(context, Formatting))
				++context.Position;
		}

		private static void ReadBreak(ParsingContext context)
		{
			ReadFormatting(context);

			if (Test(context, Comma))
			{
				++context.Position;
				ReadFormatting(context);
			}
		}

		private static bool TestBreak(ParsingContext context)
		{
			ReadSpacing(context);

			return Test(context, Breaks);
		}

		// Names
		// ----------------------------------------------------------------

		private static void ReadWord(ParsingContext context, out string @out)
		{
			int startPosition = context.Position;

			while (Test(context, Letters))
				++context.Position;

			@out = context.Source[startPosition..context.Position];
		}

		private static void ReadName(ParsingContext context, out string @out)
		{
			ReadWord(context, out string word);

			StringBuilder name = new StringBuilder(word);

			while (true)
			{
				ReadSpacing(context);

				if (Test(context, Letters))
				{
					ReadWord(context, out word);

					name.Append(' ');
					name.Append(word);
				}
				else
					break;
			}

			@out = name.ToString();
		}

		// Types
		// ----------------------------------------------------------------

		private static void ReadType(ParsingContext context, out ObganismType @out)
		{
			ReadTypeName(context, out string name);
			Checkpoint(context);
			ReadFormatting(context);

			if (TestOf(context))
			{
				Pop(context);
				ReadOf(context);
				ReadFormatting(context);

				List<ObganismType> generics;

				if (Test(context, '('))
					ReadEnclosed(
						context,
						ReadOpenParenthesis,
						(ParsingContext context, out List<ObganismType> @out) => ReadMany(context, TestType, ReadType, out @out),
						ReadCloseParenthesis,
						out generics
					);
				else
				{
					ReadType(context, out ObganismType generic);

					generics = new List<ObganismType> { generic };
				}

				@out = new ObganismType(name, generics);
			}
			else
			{
				Restore(context);
				
				@out = new ObganismType(name);
			}
		}
		
		private static bool TestType(ParsingContext context)
		{
			return Test(context, Letters) || TestEscapedOf(context);
		}

		private static void ReadTypeName(ParsingContext context, out string @out)
		{
			if (TestOf(context))
				throw new ObganismParsingException(context, "some type name", "an <<of>> keyword");

			StringBuilder name = new StringBuilder();

			if (TestEscapedOf(context))
			{
				ReadEscapedOf(context);

				name.Append("of");
			}
			else
			{
				ReadWord(context, out string word);

				name.Append(word);
			}

			while (true)
			{
				ReadSpacing(context);

				if (TestOf(context))
					break;
				else if (TestEscapedOf(context))
				{
					ReadEscapedOf(context);

					name.Append(" of");
				}
				else if (Test(context, Letters))
				{
					ReadWord(context, out string word);

					name.Append(' ');
					name.Append(word);
				}
				else
					break;
			}

			@out = name.ToString();
		}

		private static void ReadOf(ParsingContext context)
		{
			context.Position += 2;
		}

		private static bool TestOf(ParsingContext context)
		{
			return Test(context, 3)
			    && context.Source.Substring(context.Position, 2).Equals("of", StringComparison.OrdinalIgnoreCase)
			    && !Letters.Contains(context.Source[context.Position + 2]);
		}

		private static void ReadEscapedOf(ParsingContext context)
		{
			context.Position += 3;
		}

		private static bool TestEscapedOf(ParsingContext context)
		{
			return Test(context, '\\');
		}

		// Properties
		// ----------------------------------------------------------------

		private static void ReadProperty(ParsingContext context, out ObganismProperty @out)
		{
			ReadName(context, out string name);
			ReadFormatting(context);
			ReadColon(context);
			ReadFormatting(context);
			ReadType(context, out ObganismType type);

			@out = new ObganismProperty(name, type);
		}

		private static bool TestProperty(ParsingContext context)
		{
			return Test(context, Letters);
		}

		// Objects
		// ----------------------------------------------------------------

		private static void ReadObject(ParsingContext context, out ObganismObject @out)
		{
			ReadType(context, out ObganismType type);
			Checkpoint(context);
			ReadFormatting(context);
			
			if (Test(context, '{'))
			{
				Pop(context);
				ReadEnclosed(
					context,
					ReadOpenBrace,
					(ParsingContext context, out List<ObganismProperty> @out) => ReadMany(context, TestProperty, ReadProperty, out @out),
					ReadCloseBrace,
					out List<ObganismProperty> properties
				);

				@out = new ObganismObject(type, properties);
			}
			else
			{
				Restore(context);
				
				@out = new ObganismObject(type);
			}
		}

		// Punctuation
		// ----------------------------------------------------------------

		private static void ReadColon(ParsingContext context)
		{
			++context.Position;
		}

		private static void ReadOpenParenthesis(ParsingContext context)
		{
			++context.Position;
		}

		private static void ReadCloseParenthesis(ParsingContext context)
		{
			++context.Position;
		}

		private static void ReadOpenBrace(ParsingContext context)
		{
			++context.Position;
		}

		private static void ReadCloseBrace(ParsingContext context)
		{
			++context.Position;
		}

		// Utilities and Refactored Shit
		// ----------------------------------------------------------------

		private static void Checkpoint(ParsingContext context) => context.Checkpoints.Push(context.Position);
		private static void Restore(ParsingContext context) => context.Position = context.Checkpoints.Pop();
		private static void Pop(ParsingContext context) => context.Checkpoints.Pop();
		
		private static bool Test(ParsingContext context) => context.Position < context.Source.Length;
		private static bool Test(ParsingContext context, int count) => context.Position + count < context.Source.Length;
		private static bool Test(ParsingContext context, char match) => Test(context) && context.Source[context.Position] == match;
		private static bool Test(ParsingContext context, string oneOf) => Test(context) && oneOf.Contains(context.Source[context.Position]);

		private delegate void Reader(ParsingContext context);
		private delegate void Reader<T>(ParsingContext context, out T @out);

		private delegate bool Tester(ParsingContext context);

		private static void ReadEnclosed<T>(ParsingContext context, Reader readOpen, Reader<T> readInner, Reader readClose, out T @out)
		{
			readOpen(context);
			ReadFormatting(context);

			readInner(context, out @out);

			ReadFormatting(context);
			readClose(context);
		}

		/// <summary> Read 0, 1, or more items. </summary>
		private static void ReadAny<T>(ParsingContext context, Tester testItem, Reader<T> readItem, out List<T> @out)
		{
			if (testItem(context))
				ReadMany(context, testItem, readItem, out @out);
			else
				@out = new List<T>(0);
		}

		/// <summary> Read 1, or more items. </summary>
		private static void ReadMany<T>(ParsingContext context, Tester testItem, Reader<T> readItem, out List<T> @out)
		{
			readItem(context, out T item);

			@out = new List<T> { item };

			while (true)
			{
				ReadSpacing(context);

				if (Test(context, LineBreaks))
					ReadFormatting(context);

				if (Test(context, ','))
				{
					++context.Position;
					ReadFormatting(context);
					readItem(context, out item);

					@out.Add(item);
				}
				else if (testItem(context))
				{
					readItem(context, out item);

					@out.Add(item);
				}
				else
					break;
			}
		}

		private const string Letters = "etaoinsrhdlucmfywgpbvkxqjzETAOINSRHDLUCMFYWGPBVKXQJZ"; // Ordered by usage frequency in English.
		private const string Spaces = " \t";
		private const string LineBreaks = "\n\r";
		private const string Formatting = " \t\n\r";
		private const char Comma = ',';
		private const string Breaks = "\n\r,";

		private const string ValidCharacters = Letters + Spaces + Breaks + "\\:(){}"; // OLD?


		// TODO: add assertions and error messages
		// TODO: remove old shit


		// OLD SHIT
		// vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
		/*
		private static void SkipWhile(ParsingContext context, Predicate<char> predicate)
		{
			while (Test(context, predicate))
				++context.Position;
		}

		private static void SkipSpaces(ParsingContext context)
		{
			SkipWhile(context, Spaces.Contains);
		}

		private static void SkipFormatting(ParsingContext context)
		{
			SkipWhile(context, Formatting.Contains);
		}
		
		//*
		private static void SkipBreak(ParsingContext context)
		{
			SkipFormatting(context);

			if (Test(context, @char => @char == ','))
			{
				++context.Position;
				SkipFormatting(context);
			}
		}
		/**/
		/*
		private static void SkipOf(ParsingContext context)
		{
			context.Position += 2;
		}

		private static void SkipOpenParenthesis(ParsingContext context)
		{
			++context.Position;
		}

		private static void SkipCloseParenthesis(ParsingContext context)
		{
			++context.Position;
		}

		private static void SkipOpenBrace(ParsingContext context)
		{
			++context.Position;
		}

		private static void SkipCloseBrace(ParsingContext context)
		{
			++context.Position;
		}

		private static void SkipComma(ParsingContext context)
		{
			++context.Position;
		}

		private static void SkipColon(ParsingContext context)
		{
			++context.Position;
		}

		private static bool Test(ParsingContext context, Predicate<char> predicate)
		{
			return context.Position < context.Source.Length
			    && predicate(context.Source[context.Position]);
		}

		private static bool TestLetter(ParsingContext context)
		{
			return Test(context, Letters.Contains);
		}

		private static bool TestOf(ParsingContext context)
		{
			return context.Position + 3 < context.Source.Length
			    && context.Source.Substring(context.Position, 2).Equals("of", StringComparison.OrdinalIgnoreCase)
			    && !Letters.Contains(context.Source[context.Position + 2]);
		}

		private static bool TestEscapedOf(ParsingContext context)
		{
			return context.Position + 3 < context.Source.Length
			    && context.Source.Substring(context.Position, 3).Equals("\\of", StringComparison.OrdinalIgnoreCase);
		}

		/*
		private static bool TestBreak(ParsingContext context)
		{
			int lookupPosition = context.Position;

			while (lookupPosition < context.Source.Length && Spaces.Contains(context.Source[lookupPosition]))
				++lookupPosition;

			return lookupPosition < context.Source.Length
				&& Breaks.Contains(context.Source[lookupPosition]);
		}
		/**/
		/*
		private static bool TestOpenParenthesis(ParsingContext context)
		{
			return Test(context, @char => @char == '(');
		}

		private static bool TestCloseParenthesis(ParsingContext context)
		{
			return Test(context, @char => @char == ')');
		}

		private static bool TestOpenBrace(ParsingContext context)
		{
			return Test(context, @char => @char == '{');
		}

		private static bool TestCloseBrace(ParsingContext context)
		{
			return Test(context, @char => @char == '}');
		}

		private static bool TestComma(ParsingContext context)
		{
			return Test(context, @char => @char == ',');
		}

		private static bool TestColon(ParsingContext context)
		{
			return Test(context, @char => @char == ':');
		}

		private static void AssertValidCharacter(ParsingContext context, string expectation)
		{
			if (context.Position < context.Source.Length && !ValidCharacters.Contains(context.Source[context.Position]))
				throw new ObganismParsingException(context, expectation);
		}
		*/
	}
}
