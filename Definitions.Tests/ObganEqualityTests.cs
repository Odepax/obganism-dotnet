using NUnit.Framework;

namespace Obganism.Definitions.Tests
{
	public class ObganEqualityTests
	{
		[Test]
		[TestCaseSource(nameof(EqualSamples))]
		public void These_obgans_are_equal(object left, object right)
		{
			Assert.IsTrue(left.Equals(right));
			Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
		}

		public static readonly object[] EqualSamples =
		{
			new[]
			{
				new Obgan(new Type("t")),
				new Obgan(new Type("t"))
			},
			new[]
			{
				new Obgan(new Type("t", new Type("a"))),
				new Obgan(new Type("t", new Type("a")))
			},
			new[]
			{
				new Obgan(new Type("t"), new Property("a", new Type("a"))),
				new Obgan(new Type("t"), new Property("a", new Type("a")))
			},
			new[]
			{
				new Obgan(new Type("t"), new Property("a", new Type("a")), new Property("b", new Type("b"))),
				new Obgan(new Type("t"), new Property("a", new Type("a")), new Property("b", new Type("b")))
			}
		};

		[Test]
		[TestCaseSource(nameof(DifferentSamples))]
		public void These_obgans_are_not_equal(object left, object right)
		{
			Assert.IsFalse(left.Equals(right));
			Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
		}

		public static readonly object[] DifferentSamples =
		{
			new[]
			{
				new Obgan(new Type("t")),
				new Obgan(new Type("u"))
			},
			new[]
			{
				new Obgan(new Type("t", new Type("a"))),
				new Obgan(new Type("t", new Type("b")))
			},
			new[]
			{
				new Obgan(new Type("t"), new Property("a", new Type("a"))),
				new Obgan(new Type("t"), new Property("b", new Type("a")))
			},
			new[]
			{
				new Obgan(new Type("t"), new Property("a", new Type("a"))),
				new Obgan(new Type("t"), new Property("a", new Type("b")))
			},
			new[]
			{
				new Obgan(new Type("t"), new Property("a", new Type("a")), new Property("b", new Type("b"))),
				new Obgan(new Type("t"), new Property("b", new Type("b")), new Property("a", new Type("a")))
			}
		};
	}
}
