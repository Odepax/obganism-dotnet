using NUnit.Framework;

namespace Obganism.Definitions.Tests
{
	public class PropertyEqualityTests
	{
		[Test]
		[TestCaseSource(nameof(EqualSamples))]
		public void These_properties_are_equal(object left, object right)
		{
			Assert.IsTrue(left.Equals(right));
			Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
		}

		public static readonly object[] EqualSamples =
		{
			new[]
			{
				new Property("n", new Type("t")),
				new Property("n", new Type("t"))
			},
			new[]
			{
				new Property("n", new Type("t", new Type("a"))),
				new Property("n", new Type("t", new Type("a")))
			}
		};

		[Test]
		[TestCaseSource(nameof(DifferentSamples))]
		public void These_properties_are_not_equal(object left, object right)
		{
			Assert.IsFalse(left.Equals(right));
			Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
		}

		public static readonly object[] DifferentSamples =
		{
			new[]
			{
				new Property("n", new Type("t")),
				new Property("m", new Type("t"))
			},
			new[]
			{
				new Property("n", new Type("t")),
				new Property("n", new Type("u"))
			},
			new[]
			{
				new Property("n", new Type("t", new Type("a"))),
				new Property("n", new Type("t", new Type("b")))
			}
		};
	}
}
