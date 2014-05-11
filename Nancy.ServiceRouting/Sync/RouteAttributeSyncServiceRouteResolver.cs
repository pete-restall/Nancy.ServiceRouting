using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting.Sync
{
	public class RouteAttributeSyncServiceRouteResolver: RouteAttributeServiceRouteResolver
	{
		protected override IEnumerable<MethodInfo> GetServiceMethodsFrom(Type serviceType)
		{
			return serviceType.AllPublicInstanceMethods().Where(x => x.NumberOfParameters() == 1 && !x.IsAsyncCallable());
		}
	}
}
