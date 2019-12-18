using System.Collections.Generic;
using System.Linq;
using Obganism.Definitions;
using Superpower;
using Superpower.Parsers;
using Superpower.Display;
using Superpower.Tokenizers;
using Superpower.Model;

namespace Obganism.Parsing
{
	internal static class Grammar
	{
		internal enum ObganismToken
		{
			None,

			[Token(Category = "word", Description = "part of an identifier's name", Example = "camel")] Word,
			[Token(Category = "punctuation", Description = "casts the 'of' keyword down to standard word in type names", Example = "\\")] Escape,

			[Token(Category = "punctuation", Description = "type introducer", Example = ":")] Colon,

			[Token(Category = "punctuation", Description = "element separator, or decorative formatting", Example = "\n\r")] LineBreak,
			[Token(Category = "punctuation", Description = "element separator", Example = ",")] Comma,

			[Token(Category = "punctuation", Description = "list introducer", Example = "(")] OpenParen,
			[Token(Category = "punctuation", Description = "list terminator", Example = ")")] CloseParen,

			[Token(Category = "punctuation", Description = "block introducer", Example = "{")] OpenBrace,
			[Token(Category = "punctuation", Description = "block terminator", Example = "}")] CloseBrace
		}

		private static readonly TextParser<char[]> Formatting = Character
			.In(' ', '\t', '\n', '\r')
			.Many();

		private const string UnaccentuatedLetters = "etaoinsrhdlucmfywgpbvkxqjzETAOINSRHDLUCMFYWGPBVKXQJZ";

		private static readonly TextParser<string> Word = Character
			.In(UnaccentuatedLetters.ToCharArray())
			.AtLeastOnce()
			.Select(chars => new string(chars));

		internal static readonly Tokenizer<ObganismToken> Tokenizer = new TokenizerBuilder<ObganismToken>()
			.Ignore(Character.In(' ', '\t').AtLeastOnce())

			.Match(Word, ObganismToken.Word)
			.Match(Character.EqualTo('\\'), ObganismToken.Escape)

			.Match(Character.EqualTo(':').Between(Formatting, Formatting), ObganismToken.Colon)
			.Match(Character.EqualTo('(').Between(Formatting, Formatting), ObganismToken.OpenParen)
			.Match(Character.EqualTo('{').Between(Formatting, Formatting), ObganismToken.OpenBrace)

			.Match(Formatting.IgnoreThen(Character.EqualTo(')')), ObganismToken.CloseParen)
			.Match(Formatting.IgnoreThen(Character.EqualTo('}')), ObganismToken.CloseBrace)

			.Match(Character.EqualTo(',').Between(Formatting, Formatting), ObganismToken.Comma)
			.Match(Character.In('\n', '\r').AtLeastOnce(), ObganismToken.LineBreak)

			.Build();

		private static readonly TokenListParser<ObganismToken, Token<ObganismToken>> Break = (
			Token.EqualTo(ObganismToken.Comma)
		).Or(
			Token.EqualTo(ObganismToken.LineBreak)
		);

		private static readonly TokenListParser<ObganismToken, string> Name = (
			Token.EqualTo(ObganismToken.Word).Apply(Word)
				.AtLeastOnce()
				.Select(words => string.Join(' ', words))
		);

		private static readonly TokenListParser<ObganismToken, Type> Type = (
			from name in (
				Token.EqualTo(ObganismToken.Word).Apply(Word).Where(word => !word.Equals("of"))
			).Or(
				Token.EqualTo(ObganismToken.Escape).IgnoreThen(
					Token.EqualTo(ObganismToken.Word).Apply(Word).Where(word => word.Equals("of"))
				)
			)
				.AtLeastOnce()
				.Select(words => string.Join(' ', words))
			select new Type(name)
		);

		private static readonly TokenListParser<ObganismToken, Property> Property = (
			from name in Name
			from _ in Token.EqualTo(ObganismToken.Colon)
			from type in Type
			select new Property(name, type)
		);

		private static readonly TokenListParser<ObganismToken, Obgan> Obgan = (
			from type in Type
			from properties in Property.AtLeastOnceDelimitedBy(Break)
				.Between(
					Token.EqualTo(ObganismToken.OpenParen),
					Token.EqualTo(ObganismToken.CloseParen)
				)
				.OptionalOrDefault(new Property[0])
			select new Obgan(type, properties)
		);

		internal static readonly TokenListParser<ObganismToken, IReadOnlyList<Obgan>> Obganism = (
			from obgans in Obgan.ManyDelimitedBy(Break)
				.Between(
					Token.EqualTo(ObganismToken.LineBreak).Optional(),
					Token.EqualTo(ObganismToken.LineBreak).Optional()
				)
			select new List<Obgan>(obgans) as IReadOnlyList<Obgan>
		).AtEnd();

		//private static readonly TextParser<Type> Type = (
		//	from name in TypeName
		//	from generics in Formatting
		//		.IgnoreThen(Span.EqualTo("of"))
		//		.IgnoreThen(Formatting)
		//		.IgnoreThen(
		//			(
		//				Parse.Ref(() => Type)
		//					.Select(generic => new[] { generic } as IEnumerable<Type>)
		//			).Or(
		//				Parse.Ref(() => Type)
		//					.ManyDelimitedByIgnoreTrailing(Break)
		//					.Between(Formatting, Formatting)
		//					.Between(Character.EqualTo('('), Character.EqualTo(')'))
		//			)
		//		)
		//		.Try()
		//		.OptionalOrDefault(new Definitions.Type[0])
		//	select new Type(name, generics)
		//);

		/*
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
				.IgnoreThen(
					(
						Parse.Ref(() => Type)
							.Select(generic => new[] { generic } as IEnumerable<Type>)
					).Or(
						Parse.Ref(() => Type)
							.ManyDelimitedByIgnoreTrailing(Break)
							.Between(Formatting, Formatting)
							.Between(Character.EqualTo('('), Character.EqualTo(')'))
					)
				)
				.Try()
				.OptionalOrDefault(new Definitions.Type[0])
			select new Type(name, generics)
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
		*/

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
