using System.Collections.Generic;
using Obganism.Definitions;
using Sprache;

namespace Obganism.Parsing
{
	public static class ObganismParsing
	{
		public static IReadOnlyList<Obgan> ConvertFromObganism(string input)
		{
			return Grammar.Obganism.Parse(input);
		}
	}
}
