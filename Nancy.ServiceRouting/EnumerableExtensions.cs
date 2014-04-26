using System;
using System.Collections.Generic;

namespace Restall.Nancy.ServiceRouting
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
				action(item);
		}
	}
}
