using System.Collections.Generic;

namespace Obganism.Parsing
{
	internal sealed class ParsingContext
	{
		public readonly string Source;
		public int Position = 0;
		
		// TODO: Find a way to count the lines correctly,
		//       without falling into the traps of \n\r  \r\n  \n  \r
		//       Then implement these two fellows below.

		public int Line = 1;
		public int Column = 1;

		public Stack<int> Checkpoints = new Stack<int>(); // Todo: Initialize with capacity?

		public ParsingContext(string source) => Source = source;
	}
}
