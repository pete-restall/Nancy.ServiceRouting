using System;

namespace Restall.Nancy.ServiceRouting
{
	public static class RouteRegistrarFactory
	{
		public static RouteRegistrar CreateDefaultInstance(Func<Type, object> serviceFactory)
		{
			return new RouteRegistrar(
				new RouteTableBuilder(
					new AggregateServiceRouteResolver(
						new RouteAttributeSyncServiceRouteResolver(),
						new RouteAttributeAsyncServiceRouteResolver())),
				new RouteDispatchBuilder(),
				serviceFactory,
				new NancyModelServiceRequestBinder(),
				new ServiceMethodInvocationChain(
					new SyncServiceMethodInvocation(),
					new AsyncVoidServiceMethodInvocation(),
					new AsyncTaskServiceMethodInvocation(),
					new AsyncTaskOfTServiceMethodInvocation()));
		}
	}
}
