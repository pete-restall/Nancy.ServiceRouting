using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nancy;

namespace Restall.Nancy.ServiceRouting
{
	public class RouteRegistrar
	{
		private class RegistrationContext
		{
			public NancyModule Module;
			public Type ServiceType;
			public RouteTable RouteTable;
		}

		private readonly RouteTableBuilder routeTableBuilder;
		private readonly RouteDispatchBuilder routeDispatchBuilder;
		private readonly Func<RouteDispatchBuilder, RouteDispatchBuilder> routeDispatchContext;

		public RouteRegistrar(
			RouteTableBuilder routeTableBuilder,
			RouteDispatchBuilder routeDispatchBuilder,
			Func<Type, object> serviceFactory,
			IServiceRequestBinder requestMessageBinder,
			IServiceMethodInvocation serviceMethodInvocation):
			this(
				routeTableBuilder,
				routeDispatchBuilder,
				builder => builder
					.WithServiceFactory(serviceFactory)
					.WithServiceMethodInvocation(serviceMethodInvocation)
					.WithRequestMessageBinder(requestMessageBinder))
		{
		}

		private RouteRegistrar(
			RouteTableBuilder routeTableBuilder,
			RouteDispatchBuilder routeDispatchBuilder,
			Func<RouteDispatchBuilder, RouteDispatchBuilder> routeDispatchContext)
		{
			this.routeTableBuilder = routeTableBuilder;
			this.routeDispatchBuilder = routeDispatchBuilder;
			this.routeDispatchContext = routeDispatchContext;
		}

		public void RegisterServicesInto(NancyModule module, params Type[] serviceTypes)
		{
			this.RegisterServicesInto(module, (IEnumerable<Type>) serviceTypes);
		}

		public void RegisterServicesInto(NancyModule module, IEnumerable<Type> serviceTypes)
		{
			serviceTypes.ForEach(x => this.RegisterServiceInto(module, x));
		}

		public void RegisterServiceInto(NancyModule module, Type serviceType)
		{
			RegistrationContext context = new RegistrationContext
				{
					Module = module,
					ServiceType = serviceType,
					RouteTable = this.routeTableBuilder.ForService(serviceType).Build()
				};

			this.WireAllRoutes(context);
		}

		private void WireAllRoutes(RegistrationContext context)
		{
			var dispatchesGroupedByVerb = context.RouteTable.GetRoutesForAllVerbs()
				.Select(route => new KeyValuePair<Route, Delegate>(route, this.CreateRouteDispatch(context, route)))
				.GroupBy(dispatch => dispatch.Key.Verb)
				.ToArray();

			dispatchesGroupedByVerb.ForEach(dispatchesForVerb =>
			{
				var routeBuilder = new NancyModule.RouteBuilder(dispatchesForVerb.Key, context.Module);
				WireSyncRoutes(routeBuilder, dispatchesForVerb);
				WireAsyncRoutes(routeBuilder, dispatchesForVerb);
			});
		}

		private Delegate CreateRouteDispatch(RegistrationContext context, Route route)
		{
			return this.routeDispatchContext(this.routeDispatchBuilder)
				.WithModule(context.Module)
				.WithServiceType(context.ServiceType)
				.WithMethod(route.Method)
				.Build();
		}

		private static void WireSyncRoutes(NancyModule.RouteBuilder nancyRoutes, IEnumerable<KeyValuePair<Route, Delegate>> dispatches)
		{
			dispatches.Where(x => x.Value is Func<object, object>)
				.ForEach(dispatch => nancyRoutes[dispatch.Key.Name, dispatch.Key.Path] = (Func<object, object>) dispatch.Value);
		}

		private static void WireAsyncRoutes(NancyModule.RouteBuilder nancyRoutes, IEnumerable<KeyValuePair<Route, Delegate>> dispatches)
		{
			dispatches.Where(x => x.Value is Func<object, CancellationToken, Task<object>>)
				.ForEach(dispatch => nancyRoutes[dispatch.Key.Name, dispatch.Key.Path, runAsync: true] =
					(Func<object, CancellationToken, Task<object>>) dispatch.Value);
		}

		public RouteRegistrar WithDispatchContext(Func<RouteDispatchBuilder, RouteDispatchBuilder> context)
		{
			return new RouteRegistrar(this.routeTableBuilder, this.routeDispatchBuilder, x => context(this.routeDispatchContext(x)));
		}
	}
}
