using System;
using System.Collections.Generic;
using FluentAssertions;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using Nancy.TinyIoc;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Restall.Nancy.ServiceRouting.Tests.Integration.CustomVerbService;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public class CustomVerbTest
	{
		private class Bootstrapper: DefaultNancyBootstrapper
		{
			protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
			{
				base.ConfigureRequestContainer(container, context);
				container.Register((ctx, args) => context);
				container.Register((ctx, args) => RouteRegistrarFactory.CreateDefaultInstance(ctx.Resolve));
			}

			protected override IEnumerable<ModuleRegistration> Modules => new[] {new ModuleRegistration(typeof(CustomVerbModule))};
		}

		[Theory, NancyAutoData]
		public void ExpectSyncNamedRouteIsResolvedCorrectlyByService(RouteRegistrar registrar, Guid token)
		{
			using (var bootstrapper = new Bootstrapper())
			{
				var browser = new Browser(bootstrapper);
				browser
					.HandleRequest("WHATEVER", "/some-sync-route/" + token, with => with.AcceptJson())
					.Body.DeserializeJson<CustomVerbResponse>()
					.EchoToken.Should().Be(token);
			}
		}

		[Theory, NancyAutoData]
		public void ExpectAsyncCustomVerbIsResolvedCorrectlyByService(RouteRegistrar registrar, Guid token)
		{
			using (var bootstrapper = new Bootstrapper())
			{
				var browser = new Browser(bootstrapper);
				browser
					.HandleRequest("FOO", "/some-async-route/" + token, with => with.AcceptJson())
					.Body.DeserializeJson<CustomVerbResponse>()
					.EchoToken.Should().Be(token);
			}
		}
	}
}
