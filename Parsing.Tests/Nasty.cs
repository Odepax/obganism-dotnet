﻿using NUnit.Framework;

namespace Obganism.Parsing.Tests
{
	class Nasty
	{
		[Test]
		public void Some_intentionally_failing_test()
		{
			Assert.Pass("PASS 100");
		}
	}
}