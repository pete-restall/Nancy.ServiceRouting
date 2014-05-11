using System;
using System.Collections.Generic;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting.Tests
{
	public static class TypeExtensions
	{
		public static IEnumerable<MethodInfo> GetAllDeclaredMethods(this Type type)
		{
			return type.GetMethods(
				BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly |
				BindingFlags.NonPublic | BindingFlags.Public |
				BindingFlags.SetProperty | BindingFlags.GetProperty);
		}
	}
}
