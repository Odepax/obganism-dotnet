using System;
using static System.Math;

namespace Obganism.Parsing
{
	/// <summary>
	///   <see cref="Context"/>, <see cref="Comment"/> and <see cref="Exception.Message"/> (inherited) are intended for debug purposes.
	///   Changes to these properties will not take part in API versionning.
	/// </summary>
	[Serializable]
	public sealed class ObganismParsingException : Exception
	{
		public int Position { get; }
		public int Line { get; }
		public int Column { get; }
		public string Context { get; }
		public string Comment { get; }

		internal ObganismParsingException(int position, int line, int column, string context, string comment)
			: base($"Invalid Obganism @{ position } L{ line } C{ column }, search for <<{ context }>> ; { comment }")
		{
			Position = position;
			Line = line;
			Column = column;
			Context = context;
			Comment = comment;
		}

		private const int ContextSpread = 11;

		internal ObganismParsingException(ParsingContext context, string expectation, string? found = null)
			: this(
				context.Position,
				context.Line,
				context.Column,
				context.Source[
					  Max(context.Position - ContextSpread, 0)
					..Min(context.Position + ContextSpread, context.Source.Length)
				],
				$"I was expecting { expectation } here, but instead, I found { found ?? (context.Position < context.Source.Length ? $"<<{ context.Source[context.Position] }>>" : "the end of the input") }."
			)
		{
			// TODO: Fix suggestions (just append to the comment message) based on what is "found instead".

			/*
				[Found] -> [Suggest]
				digit || accent || underscore -> names can only contain letters and spaces
				backslash -> \of is only valid in the context of a type name, no need to escape of in a property name
				closing paren || closing brace -> forgot extra comma || missing type after :
				open paren -> forgot of keyword
				open brace -> of keyword without generic in object type
			*/
		}

		internal ObganismParsingException(ParsingContext context, Exception innerException)
			: base($"External exception @{ context.Position } L{ context.Line } C{ context.Column }. See <<.InnerException>> for detail. This is a bug, please report it at https://github.com/Odepax/obganism-dotnet/issues.", innerException)
		{
			Position = context.Position;
			Line = context.Line;
			Column = context.Column;
			Context = context.Source[
				  Max(context.Position - ContextSpread, 0)
				..Min(context.Position + ContextSpread, context.Source.Length)
			];
			Comment = string.Empty;
		}
	}
}
