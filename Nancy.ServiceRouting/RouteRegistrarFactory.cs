using System;

namespace Restall.Nancy.ServiceRouting
{
	public static class RouteRegistrarFactory
	{
		public static RouteRegistrar CreateDefaultInstance(Func<Type, object> serviceFactory)
		{
			return new RouteRegistrar(
				new RouteTableBuilder(new RouteAttributeSyncServiceRouteResolver()),
				new RouteDispatchBuilder(),
				serviceFactory,
				new NancyModelServiceRequestBinder(),
				new SyncServiceMethodInvocation());
		}
	}
}
