using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Restall.Nancy.ServiceRouting
{
	internal static class MethodInfoExtensions
	{
		public static bool IsShadowed(this MethodInfo method, Type mostDerivedType)
		{
			return !method.IsVirtual && method.DeclaringType != mostDerivedType && method.GetMethodsWithSameNameAndSignature(mostDerivedType).Any();
		}

		private static IEnumerable<MethodInfo> GetMethodsWithSameNameAndSignature(this MethodInfo method, Type mostDerivedType)
		{
			return mostDerivedType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(
				x =>
					x.MetadataToken != method.MetadataToken &&
					x.Name == method.Name &&
					GetMethodParameterTypes(x).SequenceEqual(method.GetMethodParameterTypes()));
		}

		private static IEnumerable<Type> GetMethodParameterTypes(this MethodInfo method)
		{
			return method.GetParameters().Select(x => x.ParameterType);
		}

		public static bool IsAsync(this MethodInfo method)
		{
			return method.GetCustomAttribute<AsyncStateMachineAttribute>() != null;
		}

		private static T GetCustomAttribute<T>(this MethodInfo method)
		{
			return method.GetCustomAttributes<T>().FirstOrDefault();
		}

		private static IEnumerable<T> GetCustomAttributes<T>(this MethodInfo method)
		{
			return method.GetCustomAttributes(typeof(T), true).Cast<T>();
		}

		public static Type TypeOfFirstParameter(this MethodInfo method)
		{
			return method.GetMethodParameterTypes().First();
		}

		public static Type TypeOfSecondParameter(this MethodInfo method)
		{
			return method.GetMethodParameterTypes().Skip(1).First();
		}
	}
}
