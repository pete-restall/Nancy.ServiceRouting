using System;
using System.Collections.Generic;

namespace Restall.Nancy.ServiceRouting.Tests
{
	public static class IntegerExtensions
	{
		public static IEnumerable<T> Select<T>(this int numberOfItems, Func<int, T> createItem)
		{
			for (int i = 0; i < numberOfItems; i++)
				yield return createItem(i);
		}
	}
}
