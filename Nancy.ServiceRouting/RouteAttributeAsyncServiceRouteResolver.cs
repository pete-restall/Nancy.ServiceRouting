using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Restall.Nancy.ServiceRouting
{
	public class RouteAttributeAsyncServiceRouteResolver: RouteAttributeServiceRouteResolver
	{
		protected override IEnumerable<MethodInfo> GetServiceMethodsFrom(Type serviceType)
		{
			return serviceType.AllPublicInstanceMethods().Where(x => IsSingleParameterWithOptionalCancellationToken(x) && x.IsAsync());
		}

		private static bool IsSingleParameterWithOptionalCancellationToken(MethodInfo method)
		{
			return
				method.GetParameters().Length == 1 ||
				(method.GetParameters().Length == 2 && method.TypeOfSecondParameter() == typeof(CancellationToken));
		}
	}
}
