﻿using System.Collections.Generic;
using System.Text;
using Obganism.Definitions;

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
		/// <seealso cref="Obgan" />
		///
		/// <exception cref="ObganismParsingException">
		///   Thrown when the <paramref name="input"/> isn't valid Obganism,
		///   or when a bug throws an unexcpected exception, in which case <see cref="System.Exception.InnerException"/> will be set.
		/// </exception>
		public static IReadOnlyList<Obgan> ConvertFromObganism(string input)
		{
			ParsingContext context = new ParsingContext(input);

			try
			{
				ReadObganism(context, out List<Obgan> output);

				return output;
			}
			catch (ObganismParsingException)
			{
				throw;
			}
			catch (System.Exception innerException)
			{
				throw new ObganismParsingException(context, innerException);
			}
		}

		private static void ReadObganism(ParsingContext context, out List<Obgan> @out)
		{
			SkipFormatting(context);
			AssertValidCharacter(context, "some type name");

			if (TestLetter(context) || TestEscapedOf(context))
			{
				ReadObgan(context, out Obgan obgan);

				List<Obgan> obgans = new List<Obgan> { obgan };

				while (true)
				{
					SkipFormatting(context);

					if (TestComma(context))
					{
						SkipComma(context);
						SkipFormatting(context);
					}

					AssertValidCharacter(context, "some type name");

					if (!(TestLetter(context) || TestEscapedOf(context)))
						break;

					ReadObgan(context, out obgan);

					obgans.Add(obgan);
				}

				SkipFormatting(context);

				@out = obgans;
			}
			else
			{
				@out = new List<Obgan>(0);
			}
		}

		private static void ReadObgan(ParsingContext context, out Obgan @out)
		{
			ReadType(context, out Type type);
			SkipFormatting(context);
			ReadProperties(context, out List<Property> properties);
			
			@out = new Obgan(type, properties);
		}

		private static void ReadType(ParsingContext context, out Type @out)
		{
			ReadTypeName(context, out string name);
			SkipFormatting(context);

			if (TestOf(context))
			{
				SkipOf(context);
				SkipFormatting(context);
				ReadGenerics(context, out List<Type> generics);

				@out = new Type(name, generics);
			}
			else
			{
				@out = new Type(name);
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

		private static void ReadGenerics(ParsingContext context, out List<Type> @out)
		{
			if (TestOpenParenthesis(context))
			{
				SkipOpenParenthesis(context);
				SkipFormatting(context);
				ReadType(context, out Type generic);

				List<Type> generics = new List<Type> { generic };

				while (true)
				{
					SkipFormatting(context);

					if (TestComma(context))
					{
						SkipComma(context);
						SkipFormatting(context);
					}

					AssertValidCharacter(context, "some type name");

					if (!(TestLetter(context) || TestEscapedOf(context)))
						break;

					ReadType(context, out generic);

					generics.Add(generic);
				}

				SkipFormatting(context);

				if (!TestCloseParenthesis(context))
					throw new ObganismParsingException(context, "a closing parenthesis");

				SkipCloseParenthesis(context);

				@out = generics;
			}
			else
			{
				ReadType(context, out Type generic);

				@out = new List<Type> { generic };
			}
		}

		private static void ReadProperties(ParsingContext context, out List<Property> @out)
		{
			if (TestOpenBrace(context))
			{
				SkipOpenBrace(context);
				SkipFormatting(context);
				ReadProperty(context, out Property property);

				List<Property> properties = new List<Property> { property };

				while (true)
				{
					SkipFormatting(context);

					if (TestComma(context))
					{
						SkipComma(context);
						SkipFormatting(context);
					}

					AssertValidCharacter(context, "some type name");

					if (!TestLetter(context))
						break;

					ReadProperty(context, out property);

					properties.Add(property);
				}

				SkipFormatting(context);

				if (!TestCloseBrace(context))
					throw new ObganismParsingException(context, "a closing brace");

				SkipCloseBrace(context);

				@out = properties;
			}
			else
			{
				@out = new List<Property>(0);
			}
		}

		private static void ReadProperty(ParsingContext context, out Property @out)
		{
			ReadPropertyName(context, out string name);
			SkipFormatting(context);

			if (!TestColon(context))
				throw new ObganismParsingException(context, $"a colon introducing the <<{ name }>> property's type");
			
			SkipColon(context);
			SkipFormatting(context);
			ReadType(context, out Type type);

			@out = new Property(name, type);
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

		private const string Letters = "etaoinsrhdlucmfywgpbvkxqjzETAOINSRHDLUCMFYWGPBVKXQJZ";
		private const string Spaces = " \t";
		private const string Breaks = "\n\r,";
		private const string Formatting = " \n\r\t";

		private const string ValidCharacters = Letters + Spaces + Breaks + "\\:(){}";

		private static void SkipWhile(ParsingContext context, System.Predicate<char> predicate)
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

		/*
		private static void SkipBreak(ParsingContext context)
		{
			SkipFormatting(context);

			if (Test(context, @char => @char == ','))
			{
				++context.Position;
				SkipFormatting(context);
			}
		}
		*/

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

		private static bool Test(ParsingContext context, System.Predicate<char> predicate)
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
			    && context.Source.Substring(context.Position, 2).Equals("of", System.StringComparison.OrdinalIgnoreCase)
			    && !Letters.Contains(context.Source[context.Position + 2]);
		}

		private static bool TestEscapedOf(ParsingContext context)
		{
			return context.Position + 3 < context.Source.Length
			    && context.Source.Substring(context.Position, 3).Equals("\\of", System.StringComparison.OrdinalIgnoreCase);
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
		*/

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
	}
}
