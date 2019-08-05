using System;
using System.Collections.Generic;
using Obganism.Definitions;
using Sprache;

namespace Obganism.Parsing
{
	public static class ObganismParsing
	{
		private const int ContextSpread = 7;

		public static IReadOnlyList<Obgan> ConvertFromObganism(string input)
		{
			IResult<IReadOnlyList<Obgan>> parsing = Grammar.Obganism.TryParse(input);

			if (parsing.WasSuccessful)
			{
				return parsing.Value;
			}
			else
			{
				int contextStartIndex = Math.Max(0, parsing.Remainder.Position - ContextSpread);
				int contextLength = Math.Min(ContextSpread * 2, parsing.Remainder.Source.Length - contextStartIndex);

				throw new ObganismParsingException(
					parsing.Remainder.Position,
					parsing.Remainder.Line,
					parsing.Remainder.Column,
					parsing.Remainder.Source.Substring(contextStartIndex, contextLength),
					parsing.Message
				);
			}
		}
	}
}
