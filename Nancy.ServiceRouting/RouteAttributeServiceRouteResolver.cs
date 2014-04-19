using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nancy.ServiceRouting
{
	public class RouteAttributeServiceRouteResolver: IServiceRouteResolver
	{
		private const BindingFlags MethodBindingFlags = BindingFlags.Instance | BindingFlags.Public;

		public IEnumerable<ServiceRoute> GetServiceRoutes(Type serviceType)
		{
			if (serviceType.IsAbstract || serviceType.IsGenericTypeDefinition)
				throw new ArgumentException("Type " + serviceType + " must be a concrete type", "serviceType");

			var serviceMethods = PublicInstanceMethodsWithSingleParameter(serviceType)
				.SelectMany(x => x.TypeOfFirstParameter().GetCustomAttributes<RouteAttribute>(), (mth, att) => new { Attribute = att, Method = mth });

			return serviceMethods.SelectMany(x => x.Attribute.Verbs.Select(y => new ServiceRoute(y, x.Attribute.Path, x.Method)));
		}

		private static IEnumerable<MethodInfo> PublicInstanceMethodsWithSingleParameter(Type serviceType)
		{
			return serviceType.GetMethods(MethodBindingFlags)
				.Where(x => x.GetParameters().Length == 1 && !IsShadowedMethod(x, serviceType))
				.Select(x => x.IsVirtual? x.GetBaseDefinition(): x);
		}

		private static bool IsShadowedMethod(MethodInfo method, Type mostDerivedType)
		{
			return !method.IsVirtual && method.DeclaringType != mostDerivedType && GetMethodsWithSameNameAndSignature(method, mostDerivedType).Any();
		}

		private static IEnumerable<MethodInfo> GetMethodsWithSameNameAndSignature(MethodInfo method, Type mostDerivedType)
		{
			return mostDerivedType.GetMethods(MethodBindingFlags).Where(
				x =>
					x.MetadataToken != method.MetadataToken &&
					x.Name == method.Name &&
					GetMethodParameterTypes(x).SequenceEqual(GetMethodParameterTypes(method)));
		}

		private static IEnumerable<Type> GetMethodParameterTypes(MethodInfo method)
		{
			return method.GetParameters().Select(x => x.ParameterType);
		}
	}
}
