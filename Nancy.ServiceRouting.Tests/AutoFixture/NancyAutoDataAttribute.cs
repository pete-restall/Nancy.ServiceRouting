﻿using System;
using Nancy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Rhino.Mocks;

namespace Restall.Nancy.ServiceRouting.Tests.AutoFixture
{
	public class NancyAutoDataAttribute: AutoDataAttribute
	{
		public NancyAutoDataAttribute(): base(new Fixture())
		{
			this.Fixture.Register(() => MockRepository.GeneratePartialMock<NancyModule>());
			this.Fixture.Register(() => new Route(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), InfoOf.Method<object>(x => x.ToString())));
			this.Fixture.Register<IServiceRouteResolver>(() => new RouteAttributeSyncServiceRouteResolver());
			this.Fixture.Register<Func<Type, object>>(() => Activator.CreateInstance);
			this.Fixture.Register<IServiceRequestBinder>(() => new NancyModelServiceRequestBinder());
			this.Fixture.Register<IServiceMethodInvocation>(() => new SyncServiceMethodInvocation());
			this.Fixture.Register(() => RouteRegistrarFactory.CreateDefaultInstance(Activator.CreateInstance));
		}
	}
}
