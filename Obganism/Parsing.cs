using static System.StringComparison;
using System.Collections.Generic;
using System.Linq;
using Sprache;
using System;
using System.Text;

namespace Obganism
{
	public class Obgan
	{
		public Type Type;
		public IReadOnlyList<Property> Properties;
	}

	public class Type
	{
		public string Name;
		public IReadOnlyList<Type> Generics;
	}

	public class Property
	{
		public string Name;
		public Type Type;
	}

	public static class Parsing
	{
		private static readonly Parser<char> Space = Parse.Chars(' ', '\t');

		private static readonly Parser<char> HardBreak = Parse.Char(',');
		private static readonly Parser<char> SoftBreak = Parse.Chars('\n', '\r');

		private static readonly Parser<char> Break = (
			from _ in SoftBreak.Many()
			from __ in HardBreak
			from ___ in SoftBreak.Many()
			select '\0'
		).Or(
			from _ in SoftBreak.AtLeastOnce()
			select '\0'
		);

		private static readonly Parser<IEnumerable<char>> Formatting = Space.Or(SoftBreak).Many();

		private static readonly Parser<char> Letter = Parse.Chars("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
		private static readonly Parser<string> Word = Letter.AtLeastOnce().Text();

		private static readonly Parser<string> Name = (
			from _ in Formatting
			from words in Word.DelimitedBy(Space.AtLeastOnce())
			from __ in Formatting
			select string.Join(' ', words)
		);

		private static readonly Parser<string> OfKeyword = (
			from _ in Formatting
			from __ in Parse.String("of")
			from ___ in Formatting
			select string.Empty
		);

		private static readonly Parser<string> TypeWord = Parse.String("\\of").Select(_ => "of").XOr(Word.Where(word => !word.Equals("of")));

		private static readonly Parser<string> TypeName = (
			from _ in Formatting
			from words in TypeWord.DelimitedBy(Space.AtLeastOnce())
			from __ in Formatting
			select string.Join(' ', words)
		);

		private static readonly Parser<Type> Type = (
			from name in TypeName
			from generics in (
				from _ in OfKeyword
				from generic in Type
				select new List<Type> { generic }
			).Optional().Select(generics => generics.GetOrElse(new List<Type>()))
			select new Type { Name = name, Generics = generics }
		);

		private static readonly Parser<Obgan> Obgan = (
			from type in Type
			select new Obgan { Type = type }
		);

		private static readonly Parser<IReadOnlyList<Obgan>> Obganism = Obgan.Many().End().Select(Enumerable.ToList);

		public static IReadOnlyList<Obgan> ConvertFromObganism(this string input)
		{
			return Obganism.Parse(input);

			var parsing = Obganism.TryParse(input);

			if (parsing.WasSuccessful)
			{
				return parsing.Value;
			}
			else
			{
				throw new InvalidObganismException
				{
					Position = parsing.Remainder.Position,
					Line = parsing.Remainder.Line,
					Column = parsing.Remainder.Column,
					Context = parsing.Remainder.Source.Substring(Math.Max(0, parsing.Remainder.Position - 5), 10),
					Comment = parsing.Message
				};
			}
		}
	}

	[Serializable]
	public sealed class InvalidObganismException : Exception
	{
		public int Position { get; set; } = 1;
		public int Line { get; set; } = 1;
		public int Column { get; set; } = 1;
		public string Context { get; set; } = string.Empty;
		public string Comment { get; set; } = string.Empty;

		public override string Message =>
			$"Invalid Obganism @ char #{ Position }, near \"{ Context }\": { Comment }";

		public string LongMessage =>
			new StringBuilder()
				.AppendLine("Invalid Obganism   c(T~Tu)")
				.AppendLine("----")
				.AppendLine($"Position: character #{ Position }, line { Line }, column { Column }")
				.AppendLine($"Context: near \"{ Context }\"")
				.AppendLine($"Comment: { Comment }")
				.ToString();
	}
}
