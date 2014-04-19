using System;
using System.Collections.Generic;

namespace Nancy.ServiceRouting
{
	public interface IServiceRouteResolver
	{
		IEnumerable<ServiceRoute> GetServiceRoutes(Type serviceType);
	}
}
