using System;
using System.Collections.Generic;
using System.Linq;

namespace Restall.Nancy.ServiceRouting
{
	public class AggregateServiceRouteResolver: IServiceRouteResolver
	{
		private readonly IServiceRouteResolver[] serviceRouteResolvers;

		public AggregateServiceRouteResolver(IEnumerable<IServiceRouteResolver> serviceRouteResolvers)
		{
			this.serviceRouteResolvers = serviceRouteResolvers.ToArray();
		}

		public IEnumerable<Route> GetServiceRoutes(Type serviceType)
		{
			return this.serviceRouteResolvers.SelectMany(x => x.GetServiceRoutes(serviceType));
		}
	}
}
