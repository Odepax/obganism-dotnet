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
			new object[] { "", new Obgan[0] },
			new object[] { " \t ", new Obgan[0] },
			new object[] { " \n ", new Obgan[0] }
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
				"pointer Of cat",
				new[] { new Obgan(new Type("pointer", new Type("cat"))) }
			},
			new object[]
			{
				"pointer OF cat",
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
			},
			new object[]
			{
				"pointer of ( cat )",
				new[] { new Obgan(new Type("pointer", new Type("cat"))) }
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
			},
			new object[]
			{
				"list of (tuple of (int, int) \n pointer of cat)",
				new[] { new Obgan(new Type("list", new Type("tuple", new Type("int"), new Type("int")), new Type("pointer", new Type("cat")))) }
			}
		};

		public static readonly object[] EmptyObganSamples =
		{
			new object[]
			{
				"cat{}",
				new[] { new Obgan(new Type("cat")) }
			},
			new object[]
			{
				"cat \t { \t }",
				new[] { new Obgan(new Type("cat")) }
			},
			new object[]
			{
				"cat \n { \n }",
				new[] { new Obgan(new Type("cat")) }
			}
		};

		public static readonly object[] SinglePropertySamples =
		{
			new object[]
			{
				"cat{name:string}",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[]
			{
				"cat \t { \t name \t : \t string \t }",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[]
			{
				"cat \n { \n name \n : \n string \n }",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[]
			{
				"cat { friend of mine : friend of mine }",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("friend of mine", new Type("friend", new Type("mine")))
					)
				}
			}
		};

		public static readonly object[] MultiPropertySamples =
		{
			new object[]
			{
				"cat { id : int,name : string }",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("id", new Type("int")),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[]
			{
				"cat { id : int \t , \t name : string }",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("id", new Type("int")),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[]
			{
				"cat { id : int \n , \n name : string }",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("id", new Type("int")),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[]
			{
				"cat { id : int \n name : string }",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("id", new Type("int")),
						new Property("name", new Type("string"))
					)
				}
			},
			new object[]
			{
				"cat { position : tuple of (int, int) \n name : string }",
				new[]
				{
					new Obgan(
						new Type("cat"),
						new Property("position", new Type("tuple", new Type("int"), new Type("int"))),
						new Property("name", new Type("string"))
					)
				}
			}
		};

		public static readonly object[] MultiObganSamples =
		{
			new object[]
			{
				"cat,dog,camel",
				new[]
				{
					new Obgan(new Type("cat")),
					new Obgan(new Type("dog")),
					new Obgan(new Type("camel"))
				}
			},
			new object[]
			{
				"cat \t , \t dog \t , \t camel",
				new[]
				{
					new Obgan(new Type("cat")),
					new Obgan(new Type("dog")),
					new Obgan(new Type("camel"))
				}
			},
			new object[]
			{
				"cat \n , \n dog \n , \n camel",
				new[]
				{
					new Obgan(new Type("cat")),
					new Obgan(new Type("dog")),
					new Obgan(new Type("camel"))
				}
			},
			new object[]
			{
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

			// I don't care about the exception not implementing .Equals().
			Assert.AreEqual(
				(expectedError.Position, /*expectedError.Line, expectedError.Column, expectedError.Context,*/ expectedError.Comment),
				(actualError.Position, /*actualError.Line, actualError.Column, actualError.Context,*/ actualError.Comment)
			);
		}

		public static readonly object[] InvalidTypeSamples =
		{
			new object[]
			{
				"4",
				new ObganismParsingException(0, 1, 1, "", "number")
			},
			new object[]
			{
				"c4t",
				new ObganismParsingException(1, 1, 1, "", "number")
			},
			new object[]
			{
				"cassé",
				new ObganismParsingException(4, 1, 1, "", "accent")
			},
			new object[]
			{
				"pointer \\to cat",
				new ObganismParsingException(8, 1, 1, "", "only of can be escaped")
			},
			new object[]
			{
				"of course",
				new ObganismParsingException(8, 1, 1, "", "leading of")
			},
			new object[]
			{
				"pointer of",
				new ObganismParsingException(8, 1, 1, "", "generic is a lie")
			},
			new object[]
			{
				"pointer of of",
				new ObganismParsingException(8, 1, 1, "", "double of")
			},
			new object[]
			{
				"pointer of \t",
				new ObganismParsingException(8, 1, 1, "", "generic is a lie")
			},
			new object[]
			{
				"pointer of \n",
				new ObganismParsingException(8, 1, 1, "", "generic is a lie")
			},
			new object[]
			{
				"pointer of {",
				new ObganismParsingException(11, 1, 1, "", "generic is a lie")
			},
			new object[]
			{
				"cat { id }",
				new ObganismParsingException(8, 1, 1, "", "no type")
			},
			new object[]
			{
				"cat { id : }",
				new ObganismParsingException(10, 1, 1, "", "no type")
			},
			new object[]
			{
				"cat { id : int, }",
				new ObganismParsingException(15, 1, 1, "", "next property is a lie")
			},
			new object[]
			{
				"cat { id : int ",
				new ObganismParsingException(8, 1, 1, "", "no closing brace")
			},
			new object[]
			{
				"cat id : int }",
				new ObganismParsingException(7, 1, 1, "", "no opening brace")
			},
			new object[]
			{
				"map of ( )",
				new ObganismParsingException(8, 1, 1, "", "generic list is a lie")
			},
			new object[]
			{
				"map of ( int, )",
				new ObganismParsingException(13, 1, 1, "", "next generic is a lie")
			},
			new object[]
			{
				"map of ( int, string",
				new ObganismParsingException(20, 1, 1, "", "no closing parenthesis")
			},
			new object[]
			{
				"map of int, string )",
				new ObganismParsingException(19, 1, 1, "", "no opening parenthesis, which results in a WTF closing paren, as the parser will take string a new new obgan")
			},
			new object[]
			{
				"cat { id : int name : string }",
				new ObganismParsingException(20, 1, 1, "", "WTF another colon")
			},
		};
	}
}
