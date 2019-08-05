using System.Collections.Generic;
using System.Linq;
using Sprache;
using Obganism.Definitions;

namespace Obganism.Parsing
{
	internal static class Grammar
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

		private static readonly Parser<char> Letter = Parse.Chars("etaoinsrhdlucmfywgpbvkxqjzETAOINSRHDLUCMFYWGPBVKXQJZ");
		private static readonly Parser<string> Word = Letter.AtLeastOnce().Text();

		private static readonly Parser<string> Name = (
			from words in Word.DelimitedBy(Space.AtLeastOnce())
			select string.Join(' ', words)
		);

		private static readonly Parser<string> OfKeyword = Parse.String("of").Text();

		private static readonly Parser<string> TypeWord = Parse
			.String("\\of").Select(_ => "of")
			.XOr(Word.Where(word => !word.Equals("of")));

		private static readonly Parser<string> TypeName = (
			from words in TypeWord.DelimitedBy(Space.AtLeastOnce())
			select string.Join(' ', words)
		);

		private static readonly Parser<Type> Type = (
			from name in TypeName
			from generics in (
				from _ in Formatting
				from __ in OfKeyword
				from ___ in Formatting
				from generic in Type
				select new List<Type> { generic }
			).Optional().Select(generics => generics.GetOrElse(new List<Type>(0)))
			select new Type(name, generics)
		);

		private static readonly Parser<Obgan> Obgan = (
			from type in Type
			select new Obgan(type)
		);

		internal static readonly Parser<IReadOnlyList<Obgan>> Obganism = (
			from _ in Formatting
			from obgans in Obgan.DelimitedBy(Break).Optional().Select(obgans => obgans.GetOrElse(new List<Obgan>(0)))
			from __ in Formatting
			select obgans.ToList()
		).End();
	}
}
