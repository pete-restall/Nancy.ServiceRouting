using System;
using System.Collections.Generic;
using System.Linq;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;

namespace Restall.Nancy.ServiceRouting.Tests
{
	public static class ArrayExtensions
	{
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			return source.OrderBy(x => Guid.NewGuid());
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
