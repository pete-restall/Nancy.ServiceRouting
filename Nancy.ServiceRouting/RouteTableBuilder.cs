using System;
using System.Linq;

namespace Restall.Nancy.ServiceRouting
{
	public class RouteTableBuilder
	{
		private readonly IServiceRouteResolver serviceRouteResolver;
		private Route[] routes;

		public RouteTableBuilder(IServiceRouteResolver serviceRouteResolver)
		{
			this.serviceRouteResolver = serviceRouteResolver;
		}

		public RouteTableBuilder ForService(Type serviceType)
		{
			this.routes = this.serviceRouteResolver.GetServiceRoutes(serviceType).ToArray();
			return this;
		}

		public RouteTable Build()
		{
			return new RouteTable(this.routes);
		}
	}
}
