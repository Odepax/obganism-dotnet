using System;

namespace Obganism.Parsing
{
	[Serializable]
	public sealed class ObganismParsingException : Exception
	{
		public int Position { get; }
		public int Line { get; }
		public int Column { get; }
		public string Context { get; }
		public string Comment { get; }

		public ObganismParsingException(int position, int line, int column, string context, string comment)
			: base($"Invalid Obganism @{ position } L{ line } C{ column }, search for << { context } >> : { comment }")
		{
			Position = position;
			Line = line;
			Column = column;
			Context = context;
			Comment = comment;
		}
	}
}
