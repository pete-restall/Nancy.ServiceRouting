using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Rhino.Mocks;

namespace Nancy.ServiceRouting.Tests.AutoFixture
{
	public class NancyAutoDataAttribute: AutoDataAttribute
	{
		public NancyAutoDataAttribute(): base(new Fixture())
		{
			this.Fixture.Register(() => MockRepository.GeneratePartialMock<NancyModule>());
			this.Fixture.Register(() => new ServiceRoute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), InfoOf.Method<object>(x => x.ToString())));
			this.Fixture.Register<IServiceRouteResolver>(() => new RouteAttributeServiceRouteResolver());
			this.Fixture.Register<Func<Type, object>>(() => Activator.CreateInstance);
			this.Fixture.Register<IServiceRequestBinder>(() => new NancyModelServiceRequestBinder());
			this.Fixture.Register<IServiceMethodInvocation>(() => new DefaultServiceMethodInvocation());
			this.Fixture.Register(() => RouteRegistrarFactory.CreateDefaultInstance(Activator.CreateInstance));
		}
	}
}
