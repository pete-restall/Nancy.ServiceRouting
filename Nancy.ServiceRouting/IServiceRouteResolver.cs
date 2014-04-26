using System;
using System.Collections.Generic;

namespace Restall.Nancy.ServiceRouting
{
	public interface IServiceRouteResolver
	{
		IEnumerable<Route> GetServiceRoutes(Type serviceType);
	}
}
