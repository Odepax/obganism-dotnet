using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Obganism.Tests
{
	static class Benchmark
	{
		[Test]
		[TestCase(2)]
		[TestCase(2)]
		[TestCase(2)]
		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		[TestCase(1000)]
		[TestCase(10000)]
		[Ignore("Manual benchmark run only.")]
		public static void Run(int count)
		{
			var obo = File.ReadAllText($"./Generated/{ count }.obo");

			var watch = Stopwatch.StartNew();
			var readOnlyLists = ObganismDocument.Parse(obo);
			watch.Stop();

			Assert.AreEqual(count, readOnlyLists.Count);
			Assert.Ignore($"{ watch.ElapsedMilliseconds }ms");
		}

		[Test]
		[TestCase(2)]
		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		[TestCase(1000)]
		[TestCase(10000)]
		[Ignore("Manual benchmark generation only.")]
		public static void Generate(int count)
		{
			var code = new StringBuilder();

			for (int i = 0; i < count; ++i)
			{
				if (i != 0) code.AppendBreak();
				code.AppendObject();
			}

			File.WriteAllText($"../../../Generated/{ count }.obo", code.ToString(), Encoding.UTF8);
		}

		static Random Random = new Random();
		static bool Toss(double chance) => Random.NextDouble() < chance;
		static int Rand(int min, int max) => Random.Next(min, max + 1);

		static void AppendObject(this StringBuilder code)
		{
			code.AppendType();
			if (Toss(0.5)) code.AppendProperties();
			if (Toss(0.5)) code.AppendModifiers();
		}

		static void AppendType(this StringBuilder code)
		{
			code.AppendTypeName();
			if (Toss(0.5)) code.AppendGenerics();
		}

		static void AppendProperties(this StringBuilder code)
		{
			code.AppendFormatting();
			code.Append('{');
			code.AppendFormatting();
			for (int i = 0, c = Rand(0, 20); i < c; ++i)
			{
				if (i != 0) code.AppendBreak();
				code.AppendProperty();
			}
			code.AppendFormatting();
			code.Append('}');
		}

		static void AppendProperty(this StringBuilder code)
		{
			code.AppendName();
			code.AppendFormatting();
			code.Append(':');
			code.AppendFormatting();
			code.AppendType();
			if (Toss(0.5)) code.AppendModifiers();
		}

		static void AppendGenerics(this StringBuilder code)
		{
			code.AppendFormatting();
			code.Append(" of ");
			code.AppendFormatting();
			if (Toss(0.5)) code.AppendType();
			else
			{
				code.Append('(');
				code.AppendFormatting();
				for (int i = 0, c = Rand(1, 3); i < c; ++i)
				{
					if (i != 0) code.AppendBreak();
					code.AppendType();
				}
				code.AppendFormatting();
				code.Append(')');
			}
		}

		static void AppendModifiers(this StringBuilder code)
		{
			code.AppendFormatting();
			code.Append("--");
			code.AppendFormatting();
			if (Toss(0.5)) code.AppendModifier();
			else
			{
				code.Append('(');
				code.AppendFormatting();
				for (int i = 0, c = Rand(1, 5); i < c; ++i)
				{
					if (i != 0) code.AppendBreak();
					code.AppendModifier();
				}
				code.AppendFormatting();
				code.Append(')');
			}
		}

		static void AppendModifier(this StringBuilder code)
		{
			code.AppendName();
			if (Toss(0.5)) code.AppendParameters();
		}

		static void AppendParameters(this StringBuilder code)
		{
			code.AppendFormatting();
			code.Append('(');
			code.AppendFormatting();
			for (int i = 0, c = Rand(1, 5); i < c; ++i)
			{
				if (i != 0) code.AppendBreak();
				switch (Rand(1, 5))
				{
					case 1: code.Append(Random.Next(427_680)); break;
					case 2: code.Append((float)Random.NextDouble() * 427_680.18f); break;
					case 3: code.AppendString(); break;
					case 4: code.AppendName(); break;
					case 5:
						code.Append(':');
						code.AppendFormatting();
						code.AppendType(); break;
				}
			}
			code.AppendFormatting();
			code.Append(')');
		}

		static void AppendString(this StringBuilder code)
		{
			code.Append('"');
			code.Append('"');
		}

		static void AppendName(this StringBuilder code)
		{
			for (int i = 0, c = Rand(1, 10); i < c; ++i)
			{
				if (i != 0) code.AppendSpaces();
				for (int j = 0, d = Rand(1, 10); j < d; ++j) code.AppendLetter();
			}
		}

		static void AppendTypeName(this StringBuilder code)
		{
			code.AppendName();
		}

		static void AppendLetter(this StringBuilder code)
		{
			code.Append("etaoinsrhdlucmywgpbvkxqjzETAOINSRHDLUCMYWGPBVKXQJZ"[Random.Next(0, 50)]);
		}

		static void AppendSpaces(this StringBuilder code)
		{
			for (int i = 0, c = Rand(1, 5); i < c; ++i)
				if (Toss(0.5)) code.Append(' ');
				else code.Append('\t');
		}

		static void AppendFormatting(this StringBuilder code)
		{
			for (int i = 0, c = Rand(0, 5); i < c; ++i)
				switch (Rand(1, 3))
				{
					case 1: code.Append(' '); break;
					case 3: code.Append('\t'); break;
					case 2: code.Append('\n'); break;
				}
		}

		static void AppendBreak(this StringBuilder code)
		{
			code.AppendFormatting();
			if (Toss(0.5)) code.Append(',');
			else code.Append('\n');
			code.AppendFormatting();
		}
	}
}
