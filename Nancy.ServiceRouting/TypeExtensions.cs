using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	internal static class TypeExtensions
	{
		public static IEnumerable<T> GetCustomAttributes<T>(this Type type)
		{
			return type.GetCustomAttributes(typeof(T), true).Cast<T>();
		}

		public static bool IsConcrete(this Type type)
		{
			return !type.IsAbstract && !type.IsGenericTypeDefinition;
		}

		public static IEnumerable<MethodInfo> AllPublicInstanceMethods(this Type type)
		{
			return type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
		}
	}
}
