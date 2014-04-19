using System;

namespace Nancy.ServiceRouting
{
	public static class RouteRegistrarFactory
	{
		public static RouteRegistrar CreateDefaultInstance(Func<Type, object> serviceFactory)
		{
			return new RouteRegistrar(
				new RouteTableBuilder(new RouteAttributeServiceRouteResolver()),
				new RouteDispatchBuilder(),
				serviceFactory,
				new NancyModelServiceRequestBinder(),
				new DefaultServiceMethodInvocation());
		}
	}
}
