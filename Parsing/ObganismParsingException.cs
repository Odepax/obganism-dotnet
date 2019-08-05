using System;
using System.Text;

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
			: base($"Invalid Obganism @ char #{ position }, near \"{ context }\": { comment }")
		{
			Position = position;
			Line = line;
			Column = column;
			Context = context;
			Comment = comment;
		}

		public string LongMessage =>
			new StringBuilder()
				.AppendLine("Invalid Obganism   c(T~Tu)")
				.AppendLine("----")
				.AppendLine($"Position: character #{ Position }")
				.AppendLine($"Line: { Line }")
				.AppendLine($"Column: { Column }")
				.AppendLine($"Context: near \"{ Context }\"")
				.AppendLine($"Comment: { Comment }")
				.ToString();
	}
}
