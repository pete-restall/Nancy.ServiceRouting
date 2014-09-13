using System;
using System.Collections.Generic;
using FluentAssertions;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using Nancy.TinyIoc;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Restall.Nancy.ServiceRouting.Tests.Integration.NamedRouteService;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public class NamedRouteTest
	{
		private class Bootstrapper: DefaultNancyBootstrapper
		{
			protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
			{
				base.ConfigureRequestContainer(container, context);
				container.Register((ctx, args) => context);
				container.Register((ctx, args) => RouteRegistrarFactory.CreateDefaultInstance(ctx.Resolve));
			}

			protected override IEnumerable<ModuleRegistration> Modules
			{
				get { return new[] {new ModuleRegistration(typeof(NamedRouteModule))}; }
			}
		}

		[Theory, NancyAutoData]
		public void ExpectSyncNamedRouteIsResolvedCorrectlyByService(RouteRegistrar registrar, Guid token)
		{
			using (var bootstrapper = new Bootstrapper())
			{
				var browser = new Browser(bootstrapper);
				browser.Get<NamedRouteResponse>("/named/sync/" + token).Uri.Should().Be("/some-named-sync-route/" + token);
			}
		}

		[Theory, NancyAutoData]
		public void ExpectAsyncNamedRouteIsResolvedCorrectlyByService(RouteRegistrar registrar, Guid token)
		{
			using (var bootstrapper = new Bootstrapper())
			{
				var browser = new Browser(bootstrapper);
				browser.Get<NamedRouteResponse>("/named/async/" + token).Uri.Should().Be("/some-named-async-route/" + token);
			}
		}
	}
}
