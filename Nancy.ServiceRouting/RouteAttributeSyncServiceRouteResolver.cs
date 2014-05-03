using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public class RouteAttributeSyncServiceRouteResolver: RouteAttributeServiceRouteResolver
	{
		protected override IEnumerable<MethodInfo> GetServiceMethodsFrom(Type serviceType)
		{
			return serviceType.AllPublicInstanceMethods().Where(x => x.GetParameters().Length == 1 && !x.IsAsync());
		}
	}
}
