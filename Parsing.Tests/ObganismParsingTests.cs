using NUnit.Framework;
using Obganism.Definitions;
using static Obganism.Parsing.ObganismParsing;

namespace Obganism.Parsing.Tests
{
	public class ObganismParsingTests
	{
		[Test]
		[TestCaseSource(nameof(EmptySamples))]
		[TestCaseSource(nameof(SimpleTypeSamples))]
		[TestCaseSource(nameof(GenericTypeSamples))]
		[TestCaseSource(nameof(MultiGenericTypeSamples))]
		[TestCaseSource(nameof(EmptyObganSamples))]
		[TestCaseSource(nameof(SinglePropertySamples))]
		[TestCaseSource(nameof(MultiPropertySamples))]
		[TestCaseSource(nameof(MultiObganSamples))]
		public void These_parsing_samples_succeed(string input, Obgan[] expectedOutput) =>
			Assert.AreEqual(expectedOutput, ConvertFromObganism(input));

		public static readonly object[] EmptySamples =
		{
			new object[] {
				"",
				new Obgan[0]
			},
			new object[] {
				" \t ",
				new Obgan[0]
			},
			new object[] {
				" \n ",
				new Obgan[0]
			}
		};

		public static readonly object[] SimpleTypeSamples =
		{
			new object[]
			{
				"cat",
				new[] { new Obgan(new Type("cat")) }
			},
			new object[]
			{
				"big cat",
				new[] { new Obgan(new Type("big cat")) }
			},
			new object[]
			{
				"big \t cat",
				new[] { new Obgan(new Type("big cat")) }
			},
			new object[]
			{
				"big \n cat",
				new[]
				{
					new Obgan(new Type("big")),
					new Obgan(new Type("cat"))
				}
			}
		};

		public static readonly object[] GenericTypeSamples =
		{
			new object[]
			{
				"pointer of cat",
				new[] { new Obgan(new Type("pointer", new Type("cat"))) }
			},
			new object[]
			{
				"pointer \t of \t cat",
				new[] { new Obgan(new Type("pointer", new Type("cat"))) }
			},
			new object[]
			{
				"pointer \n of \n cat",
				new[] { new Obgan(new Type("pointer", new Type("cat"))) }
			},
			new object[]
			{
				"pointer \\of cat",
				new[] { new Obgan(new Type("pointer of cat")) }
			},
			new object[]
			{
				"pointer \t \\of \t cat",
				new[] { new Obgan(new Type("pointer of cat")) }
			},
			new object[]
			{
				"pointer \n \\of \n cat",
				new[]
				{
					new Obgan(new Type("pointer")),
					new Obgan(new Type("of")),
					new Obgan(new Type("cat"))
				}
			},
			new object[]
			{
				"list of friend \\of mine",
				new[] { new Obgan(new Type("list", new Type("friend of mine"))) }
			},
			new object[]
			{
				"coffee",
				new[] { new Obgan(new Type("coffee")) }
			},
			new object[]
			{
				"special offer",
				new[] { new Obgan(new Type("special offer")) }
			}
		};

		public static readonly object[] MultiGenericTypeSamples =
		{
			new object[]
			{
				"list of pointer of cat",
				new[] { new Obgan(new Type("list", new Type("pointer", new Type("cat")))) }
			},
			new object[]
			{
				"map of (int,cat)",
				new[] { new Obgan(new Type("map", new Type("int"), new Type("cat"))) }
			},
			new object[]
			{
				"map of \t ( \t int \t , \t cat \t )",
				new[] { new Obgan(new Type("map", new Type("int"), new Type("cat"))) }
			},
			new object[]
			{
				"map of \n ( \n int \n , \n cat \n )",
				new[] { new Obgan(new Type("map", new Type("int"), new Type("cat"))) }
			},
			new object[]
			{
				"map of \n ( \n int \n cat \n )",
				new[] { new Obgan(new Type("map", new Type("int"), new Type("cat"))) }
			}
		};

		public static readonly object[] EmptyObganSamples =
		{
			new object[] {
				"cat{}",
				new[] { new Obgan(new Type("cat")) }
			},
			new object[] {
				"cat \t { \t }",
				new[] { new Obgan(new Type("cat")) }
			},
			new object[] {
				"cat \n { \n }",
				new[] { new Obgan(new Type("cat")) }
			}
		};

		public static readonly object[] SinglePropertySamples =
		{
			new object[] {
				"cat{name:string}",
				new[] { new Obgan(new Type("cat"), new Property("name", new Type("string"))) }
			},
			new object[] {
				"cat \t { \t name \t : \t string \t }",
				new[] { new Obgan(new Type("cat"), new Property("name", new Type("string"))) }
			},
			new object[] {
				"cat \n { \n name \n : \n string \n }",
				new[] { new Obgan(new Type("cat"), new Property("name", new Type("string"))) }
			},
			new object[] {
				"cat { friend of mine : friend of mine }",
				new[] { new Obgan(new Type("cat"), new Property("friend of mine", new Type("friend", new Type("mine")))) }
			}
		};

		public static readonly object[] MultiPropertySamples =
		{
			new object[] {
				"cat { id : int,name : string }",
				new[]
				{
					new Obgan(new Type("cat"),
						new Property("id", new Type("int")),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[] {
				"cat { id : int \t , \t name : string }",
				new[]
				{
					new Obgan(new Type("cat"),
						new Property("id", new Type("int")),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[] {
				"cat { id : int \n , \n name : string }",
				new[]
				{
					new Obgan(new Type("cat"),
						new Property("id", new Type("int")),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[] {
				"cat { id : int \n name : string }",
				new[]
				{
					new Obgan(new Type("cat"),
						new Property("id", new Type("int")),
						new Property("name", new Type("string"))
					)
				}
			}
		};

		public static readonly object[] MultiObganSamples =
		{
			new object[] {
				"cat,dog,camel",
				new[]
				{
					new Obgan(new Type("cat")),
					new Obgan(new Type("dog")),
					new Obgan(new Type("camel"))
				}
			},
			new object[] {
				"cat \t , \t dog \t , \t camel",
				new[]
				{
					new Obgan(new Type("cat")),
					new Obgan(new Type("dog")),
					new Obgan(new Type("camel"))
				}
			},
			new object[] {
				"cat \n , \n dog \n , \n camel",
				new[]
				{
					new Obgan(new Type("cat")),
					new Obgan(new Type("dog")),
					new Obgan(new Type("camel"))
				}
			},
			new object[] {
				"cat \n dog \n camel",
				new[]
				{
					new Obgan(new Type("cat")),
					new Obgan(new Type("dog")),
					new Obgan(new Type("camel"))
				}
			}
		};

		[Test]
		[TestCaseSource(nameof(InvalidTypeSamples))]
		public void These_parsing_samples_fail(string input, ObganismParsingException expectedError)
		{
			ObganismParsingException actualError = Assert.Throws<ObganismParsingException>(() =>
				ConvertFromObganism(input)
			);

			// I don't care about the exception implementing .Equals() or not.
			Assert.AreEqual(expectedError.Position, actualError.Position);
			Assert.AreEqual(expectedError.Line, actualError.Line);
			Assert.AreEqual(expectedError.Column, actualError.Column);
			Assert.AreEqual(expectedError.Context, actualError.Context);
			Assert.AreEqual(expectedError.Comment, actualError.Comment);
		}

		public static readonly object[] InvalidTypeSamples =
		{
			new object[]
			{
				"c4t",
				new ObganismParsingException(0, 1, 1, "c4t", "Names can contain only unaccentuated letters.")
			},
			new object[]
			{
				"pointer \\to cat",
				new ObganismParsingException(8, 1, 9, "ointer \\to cat", "In the context of a type, only the 'of' keyword can be escaped.")
			},
			new object[]
			{
				"pointer of",
				new ObganismParsingException(8, 1, 9, "ointer of", "In the context of a type, only the 'of' keyword can be escaped.")
			},
			new object[]
			{
				"pointer of \t",
				new ObganismParsingException(8, 1, 9, "ointer of \t", "This 'of' keyword introduces no type.")
			},
			new object[]
			{
				"pointer of \n",
				new ObganismParsingException(8, 1, 9, "ointer of \n", "This 'of' keyword introduces no type.")
			}
		};
	}
}
