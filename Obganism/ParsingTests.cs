using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Obganism;
using Sprache;

namespace ObganismTests
{
	[TestFixture]
	public class ParsingTests
	{
		private readonly Func<string, IReadOnlyList<Obgan>> ConvertFromObganism = Parsing.ConvertFromObganism;

		[Test]
		public void Parsing_nothing_returns_an_empty_list()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism(@"");

			Assert.IsEmpty(obgans);
		}

		[Test]
		public void Parsing_an_single_word_returns_a_simple_object()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism(@"
				cat
			");

			Assert.AreEqual(1, obgans.Count);
			Assert.AreEqual("cat", obgans.First().Type.Name);
		}

		[Test]
		public void Parsing_several_words_returns_a_simple_object()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism(@"
				big cat
			");

			Assert.AreEqual(1, obgans.Count);
			Assert.AreEqual("big cat", obgans.First().Type.Name);
		}

		[Test]
		public void Parsing_several_words_spearated_with_many_spaces_returns_a_simple_object()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism($@"
				big {'\t'} cat
			");

			Assert.AreEqual(1, obgans.Count);
			Assert.AreEqual("big cat", obgans.First().Type.Name);
		}

		[Test]
		public void Parsing_several_types_returns_several_objects()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism(@"
				big
				cat
			");

			Assert.AreEqual(2, obgans.Count);
			Assert.AreEqual("big", obgans.First().Type.Name);
			Assert.AreEqual("cat", obgans.Last().Type.Name);
		}

		[Test]
		public void Parsing_generics_returns_a_simple_object()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism(@"
				pointer of cat
			");

			Assert.AreEqual(1, obgans.Count);
			Assert.AreEqual("pointer", obgans.First().Type.Name);
			Assert.AreEqual(1, obgans.First().Type.Generics.Count);
			Assert.AreEqual("cat", obgans.First().Type.Generics.First().Name);
		}

		[Test]
		public void Parsing_nested_generics_returns_a_simple_object()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism(@"
				list of pointer of cat
			");

			Assert.AreEqual(1, obgans.Count);
			Assert.AreEqual("list", obgans.First().Type.Name);
			Assert.AreEqual(1, obgans.First().Type.Generics.Count);
			Assert.AreEqual("pointer", obgans.First().Type.Generics.First().Name);
			Assert.AreEqual(1, obgans.First().Type.Generics.First().Generics.Count);
			Assert.AreEqual("cat", obgans.First().Type.Generics.First().Generics.First().Name);
		}

		[Test]
		public void Parsing_escaped_of_keyword_returns_a_simple_object()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism(@"
				list of frie121nd \of mine
			");

			Assert.AreEqual(1, obgans.Count);
			Assert.AreEqual("list", obgans.First().Type.Name);
			Assert.AreEqual(1, obgans.First().Type.Generics.Count);
			Assert.AreEqual("friend of mine", obgans.First().Type.Generics.First().Name);
		}

		[Test]
		public void Parsing_escaped_word_other_than_of_returns_an_error()
		{
			Assert.Throws<ParseException>(() => ConvertFromObganism(@"
				pointer \to cat
			"));
		}


		[Test]
		public void Parsing_of_within_a_word_returns_a_simple_object()
		{
			IReadOnlyList<Obgan> obgans = ConvertFromObganism(@"
				coffee
			");

			Assert.AreEqual(1, obgans.Count);
			Assert.AreEqual("coffee", obgans.First().Type.Name);

		}

		[Test]
		public void Learning()
		{
			var parser = (
				from first in Parse.Chars('X', 'Y').Where(it => it == 'X').DelimitedBy(Parse.Char('.'))
				from _ in Parse.String(".Y.")
				from last in Parse.Char('X').DelimitedBy(Parse.Char('.'))
				select (first.ToArray(), last.ToArray())
			).End();

			var output = parser.Parse("X.X.Y.X.X.X");

			Assert.AreEqual(2, output.Item1.Length);
			Assert.AreEqual(3, output.Item2.Length);

			foreach (var item in output.Item1)
				Assert.AreEqual('X', item);

			foreach (var item in output.Item2)
				Assert.AreEqual('X', item);
		}
	}
}
