using System.Collections.Generic;
using System.Linq;
using Superpower;
using Obganism.Definitions;
using static System.StringComparison;
using Superpower.Tokenizers;
using Superpower.Parsers;
using Superpower.Model;
using Superpower.Display;

namespace Obganism.Parsing
{
	internal static class Grammar
	{
		/*
		internal enum ObganismToken
		{
			None,

			[Token(Category = "", Description = "", Example = "")] Break,

			[Token(Category = "", Description = "", Example = "")] Name,

			[Token(Category = "", Description = "", Example = "")] Of,

			[Token(Category = "punctuation", Description = "type introducer", Example = ":")] Colon,
			[Token(Category = "punctuation", Description = "list starter", Example = "(")] OpenParenthesis,
			[Token(Category = "punctuation", Description = "list terminator", Example = ")")] CloseParenthesis,
			[Token(Category = "punctuation", Description = "block starter", Example = "{")] OpenBrace,
			[Token(Category = "punctuation", Description = "block terminator", Example = "}")] CloseBrace
		}

		private static readonly TextParser<string> Name = (
			Character
				.In("etaoinsrhdlucmfywgpbvkxqjzETAOINSRHDLUCMFYWGPBVKXQJZ".ToCharArray())
				.AtLeastOnce()
				.Select(chars => new string(chars))
		);

		internal static readonly Tokenizer<ObganismToken> Tokenizer = new TokenizerBuilder<ObganismToken>()
			.Ignore(Span.WhiteSpace)

			.Match(Name, ObganismToken.Name)
			//.Match(null as TextParser<char>, ObganismToken.Of)
			//.Match(null as TextParser<char>, ObganismToken.Break)

			.Match(Character.EqualTo(':'), ObganismToken.Colon)
			.Match(Character.EqualTo('('), ObganismToken.OpenParenthesis)
			.Match(Character.EqualTo(')'), ObganismToken.CloseParenthesis)
			.Match(Character.EqualTo('{'), ObganismToken.OpenBrace)
			.Match(Character.EqualTo('}'), ObganismToken.CloseBrace)

			.Build();

		private static readonly TokenListParser<ObganismToken, Type> Type = (
			from name in Token.EqualTo(ObganismToken.Name).Apply(Name)
			select new Type(name)
		);

		private static readonly TokenListParser<ObganismToken, Obgan> Obgan = (
			from type in Type
			select new Obgan(type)
		);

		internal static readonly TokenListParser<ObganismToken, IReadOnlyList<Obgan>> Obganism = (
			from obgan in Obgan
			select new List<Obgan> { obgan } as IReadOnlyList<Obgan>
		);
		*/

		private static readonly TextParser<char[]> Formatting = Character
			.In(' ', '\t', '\n', '\r')
			.Many();

		private static readonly TextParser<char> Space = Character.In(' ', '\t');

		private static readonly TextParser<char> SoftBreak = Character.In('\n', '\r');
		private static readonly TextParser<char> HardBreak = Character.EqualTo(',');

		private static readonly TextParser<char> Break = (
			HardBreak
				.Between(Formatting, Formatting)
		).Try().Or(
			Space
				.Many()
				.IgnoreThen(SoftBreak)
				.IgnoreThen(Formatting)
				.Select(_ => ',')
		);

		private const string UnaccentuatedLetters = "etaoinsrhdlucmfywgpbvkxqjzETAOINSRHDLUCMFYWGPBVKXQJZ";

		private static readonly TextParser<char> Letter = Character.In(UnaccentuatedLetters.ToCharArray());

		private static readonly TextParser<string> Word = Letter
			.AtLeastOnce()
			.Select(chars => new string(chars));

		private static readonly TextParser<string> Name = Word
			.AtLeastOnceDelimitedByIgnoreTrailing(Space.AtLeastOnce())
			.Select(words => string.Join(' ', words));

		private static readonly TextParser<string> TypeName = Word
			.Where(word => !word.Equals("of"))
			.Or(Character
				.EqualTo('\\')
				.IgnoreThen(Span.EqualTo("of"))
				.Select(_ => "of")
			)
			.AtLeastOnceDelimitedByIgnoreTrailing(Space.AtLeastOnce())
			.Select(words => string.Join(' ', words));

		private static readonly TextParser<Type> Type = (
			from name in TypeName
			from generics in Formatting
				.IgnoreThen(Span.EqualTo("of"))
				.IgnoreThen(Formatting)
				.IgnoreThen(Parse.Ref(() => Type))
				.Try()
				.OptionalOrDefault()
			select new Type(name, generics is null ? new List<Type>(0) : new List<Type> { generics })
		);

		private static readonly TextParser<Property> Property = (
			from name in Name
			from type in Formatting
				.IgnoreThen(Character.EqualTo(':'))
				.IgnoreThen(Formatting)
				.IgnoreThen(Type)
			select new Property(name, type)
		);

		private static readonly TextParser<Obgan> Obgan = (
			from type in Type
			from properties in Formatting
				.IgnoreThen(Property
					.ManyDelimitedByIgnoreTrailing(Break)
					.Between(Formatting, Formatting)
					.Between(Character.EqualTo('{'), Character.EqualTo('}'))
				)
				.Try()
				.OptionalOrDefault(new Property[0])
			select new Obgan(type, properties)
		);

		internal static readonly TextParser<IReadOnlyList<Obgan>> Obganism = Obgan
			.ManyDelimitedByIgnoreTrailing(Break)
			.Between(Formatting, Formatting)
			.Select(obgans => obgans.ToList() as IReadOnlyList<Obgan>)
			.AtEnd();

		/// <summary>
		///
		/// So...
		///
		/// This horrible thing is actually an inlining of <c>.AtLeastOnceDelimitedBy(Space.AtLeastOnce())</c>.
		///
		/// The reason why I inlined it was to add the <c>.Try()</c>,
		/// without which the parser seems to fail on a trailing delimiter.
		///
		/// </summary>
		private static TextParser<IEnumerable<T>> AtLeastOnceDelimitedByIgnoreTrailing<T, U>(this TextParser<T> parser, TextParser<U> delimiter) =>
			parser.Then(first => delimiter
				.IgnoreThen(parser)
				.Try()
				.Many()
				.Select(rest => new[] { first }.Concat(rest))
			);

		/// <summary>
		///
		/// So...
		///
		/// One of the greatest advices I could give to any programmer is
		/// not to be afraid to take some piece of code that seems to be impossible to debug,
		/// extract it, stuff in some mocks, and debug it with unit tests.
		/// 
		/// Yeah... Certainly don't spend a whole day trying to debug the big picture... M'okey?
		///
		/// </summary>
		static TextParser<IEnumerable<T>> ManyDelimitedByIgnoreTrailing<T, U>(this TextParser<T> parser, TextParser<U> delimiter) =>
			parser.Then(first => delimiter
				.IgnoreThen(parser)
				.Try()
				.Many()
				.Select(rest => new[] { first }.Concat(rest))
			).OptionalOrDefault(new T[0]);
	}
}
