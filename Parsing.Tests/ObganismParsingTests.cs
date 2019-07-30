using System.Collections.Generic;
using NUnit.Framework;
using Obganism.Definitions;
using Obganism.Extensions.Parsing;

namespace Obganism.Parsing.Tests
{
	public class ObganismParsingTests
	{
		[Test]
		public void WuvWoo()
		{
			string input = "cat";
			IReadOnlyList<Obgan> output = input.ConvertFromObganism();

			Assert.AreEqual(new Obgan(new Type("cat")), output);
		}
	}
}
