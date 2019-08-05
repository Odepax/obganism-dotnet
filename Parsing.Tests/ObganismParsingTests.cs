using System.Collections.Generic;
using NUnit.Framework;
using Obganism.Definitions;
using Obganism.Extensions.Parsing;

namespace Obganism.Parsing.Tests
{
	public class ObganismParsingTests
	{
		[Test]
		[TestCaseSource(nameof(EmptySamples))]
		[TestCaseSource(nameof(SimpleTypeSamples))]
		[TestCaseSource(nameof(GenericTypeSamples))]
		public void These_parsing_samples_succeed(string input, IReadOnlyList<Obgan> expectedOutput) =>
			Assert.AreEqual(expectedOutput, input.ConvertFromObganism());

		public static readonly object[] EmptySamples =
		{
			new object[] {
				"",
				new List<Obgan>(0)
			},
			new object[] {
				" \n ",
				new List<Obgan>(0)
			}
		};

		public static readonly object[] SimpleTypeSamples =
		{
			new object[]
			{
				"cat",
				new List<Obgan> { new Obgan(new Type("cat")) }
			},
			new object[]
			{
				"big cat",
				new List<Obgan> { new Obgan(new Type("big cat")) }
			},
			new object[]
			{
				"big \t cat",
				new List<Obgan> { new Obgan(new Type("big cat")) }
			}
		};

		public static readonly object[] GenericTypeSamples =
		{
			new object[]
			{
				"pointer of cat",
				new List<Obgan> { new Obgan(new Type("pointer", new Type("cat"))) }
			},
			new object[]
			{
				"pointer \t of \t cat",
				new List<Obgan> { new Obgan(new Type("pointer", new Type("cat"))) }
			},
			new object[]
			{
				"pointer \n of \n cat",
				new List<Obgan> { new Obgan(new Type("pointer", new Type("cat"))) }
			},
			new object[]
			{
				"pointer \\of cat",
				new List<Obgan> { new Obgan(new Type("pointer of cat")) }
			},
			new object[]
			{
				"pointer \t \\of \t cat",
				new List<Obgan> { new Obgan(new Type("pointer of cat")) }
			},
			new object[]
			{
				"list of friend \\of mine",
				new List<Obgan> { new Obgan(new Type("list", new Type("friend of mine"))) }
			},
			new object[]
			{
				"coffee",
				new List<Obgan> { new Obgan(new Type("coffee")) }
			},
			new object[]
			{
				"offer",
				new List<Obgan> { new Obgan(new Type("offer")) }
			}
		};

		[Test]
		[TestCaseSource(nameof(InvalidTypeSamples))]
		public void These_parsing_samples_fail(string input, ObganismParsingException expectedError)
		{
			ObganismParsingException actualError = Assert.Throws<ObganismParsingException>(() =>
				input.ConvertFromObganism()
			);

			// I don't care about the exception implementing .Equals().
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
				new ObganismParsingException(1, 1, 2, "c4t", "Names can contain only unaccentuated letters.")
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
