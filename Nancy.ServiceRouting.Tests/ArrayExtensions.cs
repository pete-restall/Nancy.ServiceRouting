using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.ServiceRouting.Tests.AutoFixture;

namespace Nancy.ServiceRouting.Tests
{
	public static class ArrayExtensions
	{
		public static T[] TakeAnyNumberOfItems<T>(this T[] source)
		{
			return source.Shuffle().Take(source.AnyIndexUpToLength()).ToArray();
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			return source.OrderBy(x => Guid.NewGuid());
		}

		private static int AnyIndexUpToLength<T>(this T[] source)
		{
			return RandomNumbers.Next(0, source.Length);
		}

		public static T[] TakeAtLeastOneItem<T>(this T[] source)
		{
			return source.Shuffle().Take(source.AnyNonZeroIndexUpToLength()).ToArray();
		}

		private static int AnyNonZeroIndexUpToLength<T>(this T[] source)
		{
			return RandomNumbers.Next(1, source.Length);
		}
	}
}
