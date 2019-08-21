using System;
using System.Collections.Generic;
using Obganism.Definitions;
using Superpower;
using Superpower.Model;

namespace Obganism.Parsing
{
	public static class ObganismParsing
	{
		private const int ContextSpread = 11;

		public static IReadOnlyList<Obgan> ConvertFromObganism(string input)
		{
			/*Result<*/IReadOnlyList<Obgan>/*>*/ parsing = Grammar.Obganism.Parse(input);
			return parsing;
			/*
			if (parsing.HasValue)
			{
				return parsing.Value;
			}
			else
			{
				//int contextStartIndex = Math.Max(0, parsing.ErrorPosition.Absolute - ContextSpread);
				//int contextLength = Math.Min(ContextSpread * 2, parsing.Remainder.Source.Length - contextStartIndex);

				throw new ObganismParsingException(
					parsing.ErrorPosition.Absolute,
					parsing.ErrorPosition.Line,
					parsing.ErrorPosition.Column,
					"",//parsing.Remainder.Source.Substring(contextStartIndex, contextLength),
					parsing.ErrorMessage
				);
			}*/
		}
	}
}
