using NUnit.Framework;

namespace Obganism.Definitions.Tests
{
	public class TypeEqualityTests
	{
		[Test]
		[TestCaseSource(nameof(EqualSamples))]
		public void These_types_are_equal(object left, object right)
		{
			Assert.IsTrue(left.Equals(right));
			Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
		}

		public static readonly object[] EqualSamples =
		{
			new[]
			{
				new Type("x"),
				new Type("x")
			},
			new[]
			{
				new Type("x", new Type("a")),
				new Type("x", new Type("a"))
			},
			new[]
			{
				new Type("x", new Type("a"), new Type("b")),
				new Type("x", new Type("a"), new Type("b"))
			},
			new[]
			{
				new Type("x", new Type("a", new Type("b"))),
				new Type("x", new Type("a", new Type("b")))
			}
		};

		[Test]
		[TestCaseSource(nameof(DifferentSamples))]
		public void These_types_are_not_equal(object left, object right)
		{
			Assert.IsFalse(left.Equals(right));
			Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
		}

		public static readonly object[] DifferentSamples =
		{
			new[]
			{
				new Type("x"),
				new Type("y")
			},
			new[]
			{
				new Type("x", new Type("a")),
				new Type("x", new Type("b"))
			},
			new[]
			{
				new Type("x", new Type("a"), new Type("b")),
				new Type("x", new Type("a"), new Type("c"))
			},
			new[]
			{
				new Type("x", new Type("a", new Type("b"))),
				new Type("x", new Type("a", new Type("c")))
			}
		};
	}
}
