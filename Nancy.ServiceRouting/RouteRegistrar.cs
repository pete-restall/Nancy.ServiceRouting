﻿using System;
using System.Collections.Generic;
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

			this.WireRoutesByVerb(context, "GET", module.Get);
			this.WireRoutesByVerb(context, "PUT", module.Put);
			this.WireRoutesByVerb(context, "POST", module.Post);
			this.WireRoutesByVerb(context, "DELETE", module.Delete);
			this.WireRoutesByVerb(context, "PATCH", module.Patch);
			this.WireRoutesByVerb(context, "OPTIONS", module.Options);
		}

		private void WireRoutesByVerb(RegistrationContext context, string verb, NancyModule.RouteBuilder nancyRoutes)
		{
			context.RouteTable.GetRoutesForVerb(verb).ForEach(
				route => nancyRoutes[route.Path] = this.CreateRouteDispatch(context, route));
		}

		private Func<object, object> CreateRouteDispatch(RegistrationContext context, Route route)
		{
			return this.routeDispatchContext(this.routeDispatchBuilder)
				.WithModule(context.Module)
				.WithServiceType(context.ServiceType)
				.WithMethod(route.Method)
				.Build();
		}

		public RouteRegistrar WithDispatchContext(Func<RouteDispatchBuilder, RouteDispatchBuilder> context)
		{
			return new RouteRegistrar(this.routeTableBuilder, this.routeDispatchBuilder, x => context(this.routeDispatchContext(x)));
		}
	}
}
