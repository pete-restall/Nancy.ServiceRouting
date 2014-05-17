using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public abstract class RouteAttributeServiceRouteResolver: IServiceRouteResolver
	{
		public IEnumerable<Route> GetServiceRoutes(Type serviceType)
		{
			if (!serviceType.IsConcrete())
				throw new ArgumentException("Type " + serviceType + " must be a concrete type", "serviceType");

			return MethodsToRoutes(
				this.GetServiceMethodsFrom(serviceType)
					.Where(x => !x.IsShadowed(serviceType))
					.Select(x => x.IsVirtual? x.GetBaseDefinition(): x));
		}

		private static IEnumerable<Route> MethodsToRoutes(IEnumerable<MethodInfo> methods)
		{
			return methods
				.SelectMany(x => x.TypeOfFirstParameter().GetCustomAttributes<RouteAttribute>(), (mth, att) => new { Attribute = att, Method = mth })
				.SelectMany(x => x.Attribute.Verbs.Select(v => new Route(v, x.Attribute.Path, x.Method)));
		}

		protected abstract IEnumerable<MethodInfo> GetServiceMethodsFrom(Type serviceType);
	}
}
