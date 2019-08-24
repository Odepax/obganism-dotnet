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

		/// <summary>
		///
		/// Converts Obganism source code into CLR Obganism AST.
		///
		/// </summary>
		///
		/// <param name="input">
		///
		/// Some Obganism source code as in https://github.com/Odepax/obganism-lang/wiki.
		///
		/// </param>
		///
		/// <seealso cref="Definitions" />
		///
		/// <exception cref="ObganismParsingException">
		///
		/// Thrown when the <paramref name="input"/> isn't valid Obganism.
		///
		/// </exception>
		public static IReadOnlyList<Obgan> ConvertFromObganism(string input)
		{
			Result<IReadOnlyList<Obgan>> parsing = Grammar.Obganism.TryParse(input);

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
					parsing.ErrorPosition.Column - 1,
					"",//parsing.Remainder.Source.Substring(contextStartIndex, contextLength),
					parsing.ErrorMessage
				);
			}
		}
	}
}
