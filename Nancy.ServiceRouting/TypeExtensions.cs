using System;
using System.Collections.Generic;
using System.Linq;

namespace Nancy.ServiceRouting
{
	internal static class TypeExtensions
	{
		public static IEnumerable<T> GetCustomAttributes<T>(this Type type)
		{
			return type.GetCustomAttributes(typeof(T), true).Cast<T>();
		}
	}
}
