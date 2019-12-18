using Superpower;
using Superpower.Parsers;
using Superpower.Display;
using Superpower.Tokenizers;
using Superpower.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace Obganism.Parsing.Tests
{
	sealed class Thing : IEquatable<Thing>
	{
		public string Name;
		public string Rest;

		public bool Equals(Thing? other) => other == this || (other != null
			&& other.Name.Equals(this.Name)
			&& other.Rest.Equals(this.Rest)
		);
		
		public override string ToString() => $"Thing(Name = \"{ Name }\", Resy = \"{ Rest }\")";
	}

	public class SuperpowerLearningTests
	{
		static char[] Letters = "etaoinsrhdlucmfywgpbvkxqjzETAOINSRHDLUCMFYWGPBVKXQJZ".ToCharArray();
		static char[] NonLetters = " \t:-(){}".ToCharArray();

		//-----------------------------------------------------------------------------

		TextParser<Thing> ThingParser = (
			from name in (
				Character.In(Letters)
			).Or(
				Character.ExceptIn(NonLetters).IgnoreThen(span => Result.Empty<char>(span, "IDIOT"))
			)
				.AtLeastOnce()
			from rest in Character.AnyChar.Many().AtEnd()
			select new Thing
			{
				Name = new string(name),
				Rest = new string(rest)
			}
		);

		[Test]
		public void Valid_identifier()
		{
			Thing expected = new Thing { Name = "couCou", Rest = string.Empty };
			Thing actual = ThingParser.Parse("couCou");

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Invalid_identifier()
		{
			ParseException exception = Assert.Throws<ParseException>(() => ThingParser.Parse("Point2D"));

			Assert.AreEqual("Syntax error (line 1, column 7): IDIOT.", exception.Message);

			Assert.AreEqual(6, exception.ErrorPosition.Absolute);
			Assert.AreEqual(1, exception.ErrorPosition.Line);
			Assert.AreEqual(7, exception.ErrorPosition.Column);
		}

		[Test]
		public void Trailling_space()
		{
			Thing expected = new Thing { Name = "Point", Rest = " " };
			Thing actual = ThingParser.Parse("Point ");

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Invalid_but_after_space()
		{
			Thing expected = new Thing { Name = "Point", Rest = " 2D" };
			Thing actual = ThingParser.Parse("Point 2D");

			Assert.AreEqual(expected, actual);
		}



		// ------------------------------------------

		enum ThingToken
		{
			None, Identifier
		}

		Tokenizer<ThingToken> Tokenizer = new TokenizerBuilder<ThingToken>()
			.Ignore(Span.WhiteSpace)
			.Match(Character.In(Letters).AtLeastOnce(), ThingToken.Identifier)
			.Build();

		TokenListParser<ThingToken, Thing> ThingTokenParser = (
			from name in Token.EqualTo(ThingToken.Identifier).Apply(Character.In(Letters).IgnoreThen(Character.EqualTo('A')))
			select new Thing
			{
				Name = new string(name + ""),
				Rest = new string("")
			}
		);



		[Test]
		public void Valid_identifier__TOKENS()
		{
			Thing expected = new Thing { Name = "couCou", Rest = string.Empty };
			Thing actual = ThingTokenParser.Parse(Tokenizer.Tokenize("couCou"));

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Invalid_identifier__TOKENS()
		{
			ParseException exception = Assert.Throws<ParseException>(() => ThingTokenParser.Parse(Tokenizer.Tokenize("Point2D")));

			Assert.AreEqual("Syntax error (line 1, column 7): IDIOT.", exception.Message);

			Assert.AreEqual(6, exception.ErrorPosition.Absolute);
			Assert.AreEqual(1, exception.ErrorPosition.Line);
			Assert.AreEqual(6, exception.ErrorPosition.Column);
		}

		[Test]
		public void Trailling_space__TOKENS()
		{
			Thing expected = new Thing { Name = "Point", Rest = " " };
			Thing actual = ThingTokenParser.Parse(Tokenizer.Tokenize("Point "));

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Invalid_but_after_space__TOKENS()
		{
			Thing expected = new Thing { Name = "Point", Rest = " 2D" };
			Thing actual = ThingTokenParser.Parse(Tokenizer.Tokenize("Point 2D"));

			Assert.AreEqual(expected, actual);
		}
	}
}
