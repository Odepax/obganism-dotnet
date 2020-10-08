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
		//[TestCaseSource(nameof(EmptyObganSamples))] // TODO: See ya in 1.1!
		[TestCaseSource(nameof(SinglePropertySamples))]
		[TestCaseSource(nameof(MultiPropertySamples))]
		[TestCaseSource(nameof(MultiObganSamples))]
		public void These_parsing_samples_succeed(string input, Obgan[] expectedOutput) =>
			Assert.AreEqual(expectedOutput, ConvertFromObganism(input));

		public static readonly object[] EmptySamples =
		{
			new object[] { string.Empty, new Obgan[0] },
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
				"map of (tuple of (int, int) \n pointer of cat)",
				new[] { new Obgan(new Type("map", new Type("tuple", new Type("int"), new Type("int")), new Type("pointer", new Type("cat")))) }
			},
			new object[]
			{
				"map of int,cat",
				new[]
				{
					new Obgan(new Type("map", new Type("int"))),
					new Obgan(new Type("cat"))
				}
			}
		};

		/*
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
		*/

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
		public void These_parsing_samples_fail()
		{
		}

		private static string Comment(string expectation, string found) =>
			$"I was expecting { expectation } here, but instead, I found { found }.";

		public static readonly object[] InvalidSimpleTypeSamples =
		{
			
		};

		public static readonly object[] InvalidGenericTypeSamples =
		{
			new object[]
			{
				"pointer of of cat",
				(11, 1, 1, Comment("some type name", "an <<of>> keyword"))
			},
			new object[]
			{
				"of course",
				(0, 1, 1, Comment("some type name", "an <<of>> keyword"))
			},
			new object[]
			{
				"pointer of",
				(10, 1, 1, Comment("some type name", "the end of the input"))
			},
			new object[]
			{
				"pointer of \t",
				(12, 1, 1, Comment("some type name", "the end of the input"))
			},
			new object[]
			{
				"pointer of \n",
				(12, 1, 1, Comment("some type name", "the end of the input"))
			},
			new object[]
			{
				"pointer of {}",
				(11, 1, 1, Comment("some type name", "<<{>>"))
			},
			new object[]
			{
				"pointer \\to cat",
				(8, 1, 1, "only an occurence of the <<of>> keyword can be escaped")
			}
		};

		public static readonly object[] InvalidMultiGenericTypeSamples =
		{
			new object[]
			{
				"map of { string, int }",
				(4, 1, 1, Comment("an open parenthesis", "<<{>>"))
			},
			new object[]
			{
				"map of ( )",
				(9, 1, 1, Comment("some type name", "<<)>>"))
			},
			new object[]
			{
				"map of ( int, )",
				(14, 1, 1, Comment("some type name", "<<)>>"))
			},
			new object[]
			{
				"map of ( int, string",
				(20, 1, 1, Comment("a closing parenthesis", "the end of the input"))
			},
			new object[]
			{
				"map of int, string )", // <<string>> is considered as  a new object.
				(19, 1, 1, Comment("some type name", "<<)>>"))
			},
			new object[]
			{
				"map of (tuple of (int, int) \t string)",
				(31, 1, 1, Comment("a closing parenthesis", "<<s>>"))
			}
		};

		public static readonly object[] InvalidEmptyObganSamples =
		{
			new object[]
			{
				"cat { \t }",
				(8, 1, 1, Comment("some property name", "<<}>>"))
			},
			new object[]
			{
				"cat { \n }",
				(8, 1, 1, Comment("some property name", "<<}>>"))
			}
		};

		public static readonly object[] InvalidPropertySamples =
		{
			new object[]
			{
				"cat ( name : string )",
				(4, 1, 1, Comment("some type name", "<<(>>")) // Would have prefered <<Expected an open brace>>...
			},
			new object[]
			{
				"cat { map : 4tlas }",
				(12, 1, 1, Comment("some type name", "<<4>>"))
			},
			new object[]
			{
				"cat { friend : c4t }",
				(16, 1, 1, Comment("some type name", "<<4>>"))
			},
			new object[]
			{
				"cat { c4r : car }",
				(7, 1, 1, Comment("some property name", "<<4>>"))
			},
			new object[]
			{
				"cat { : int }",
				(6, 1, 1, Comment("some property name", "<<:>>"))
			},
			new object[]
			{
				"cat { id }",
				(9, 1, 1, Comment("a colon introducing the <<id>> property's type", "<<}>>"))
			},
			new object[]
			{
				"cat { : }",
				(6, 1, 1, Comment("some property name", "<<:>>"))
			},
			new object[]
			{
				"cat { id : }",
				(11, 1, 1, Comment("some type name", "<<}>>"))
			},
			new object[]
			{
				"cat { id : int ",
				(15, 1, 1, Comment("a closing brace", "the end of the input"))
			},
			new object[]
			{
				"cat id : int }",
				(7, 1, 1, Comment("an opening brace", "<<:>>"))
			}
		};

		public static readonly object[] InvalidMultiPropertySamples =
		{
			new object[]
			{
				"cat { id : int, }",
				(16, 1, 1, Comment("some property name", "<<}>>"))
			},
			new object[]
			{
				"cat { id : int name : string }",
				(20, 1, 1, Comment("some type name", "<<:>>"))
			},
			new object[]
			{
				"cat { friends : map of (cat, float) \t name : string }",
				(39, 1, 1, Comment("a closing brace", "<<n>>"))
			}
		};

		public static readonly object[] InvalidMultiObganSamples =
		{
			new object[]
			{
				"cat \n 4tlas",
				(6, 1, 1, Comment("some type name", "<<4>>"))
			},
			new object[]
			{
				"tuple of (int, string) \t cat",
				(26, 1, 1, Comment("some break", "<<c>>"))
			},
			new object[]
			{
				"cat { id : int } \t dog",
				(20, 1, 1, Comment("some break", "<<d>>"))
			},
			new object[]
			{
				"cat,",
				(4, 1, 1, Comment("some type name", "the end of the input"))
			},
			new object[]
			{
				",cat",
				(0, 1, 1, Comment("some type name", "<<,>>"))
			}
		};
	}
}
