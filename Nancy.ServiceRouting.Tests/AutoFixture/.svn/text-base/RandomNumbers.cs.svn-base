using System;

namespace Nancy.ServiceRouting.Tests.AutoFixture
{
	public static class RandomNumbers
	{
		[ThreadStatic]
		private static Random random;

		public static int Next(int min, int halfOpenMax)
		{
			return Generator.Next(min, halfOpenMax);
		}

		private static Random Generator
		{
			get { return random ?? (random = new Random()); }
		}
	}
}
