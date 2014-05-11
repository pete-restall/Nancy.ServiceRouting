using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Restall.Nancy.ServiceRouting.Async
{
	public class RouteAttributeAsyncServiceRouteResolver: RouteAttributeServiceRouteResolver
	{
		protected override IEnumerable<MethodInfo> GetServiceMethodsFrom(Type serviceType)
		{
			return serviceType.AllPublicInstanceMethods().Where(x => IsSingleParameterWithOptionalCancellationToken(x) && x.IsAsyncCallable());
		}

		private static bool IsSingleParameterWithOptionalCancellationToken(MethodInfo method)
		{
			return
				method.NumberOfParameters() == 1 ||
				(method.NumberOfParameters() == 2 && method.TypeOfSecondParameter() == typeof(CancellationToken));
		}
	}
}
