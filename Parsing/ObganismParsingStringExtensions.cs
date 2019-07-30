using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Obganism.Definitions;
using Obganism.Parsing;

namespace Obganism.Extensions.Parsing
{
	public static class ObganismParsingStringExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<Obgan> ConvertFromObganism(this string input) =>
			ObganismParsing.ConvertFromObganism(input);
	}
}
