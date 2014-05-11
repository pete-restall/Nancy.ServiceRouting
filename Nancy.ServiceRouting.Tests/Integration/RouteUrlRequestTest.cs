using System;
using FluentAssertions;
using Nancy.Testing;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Restall.Nancy.ServiceRouting.Tests.Integration.CalculatorService;
using Restall.Nancy.ServiceRouting.Tests.Integration.EchoService;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public class RouteUrlRequestTest
	{
		[Theory]
		[NancyInlineAutoData("GET")]
		[NancyInlineAutoData("DELETE")]
		[NancyInlineAutoData("OPTIONS")]
		public void ExpectRouteIsWiredUpAndResolvedCorrectly(string verb, RouteRegistrar registrar, Guid token)
		{
			var browser = new Browser(with => with.Module(new EchoModule(registrar)));
			browser.SendUrlRequest<EchoResponse>(verb, "/echo/" + token).TokenEcho.Should().Be(token);
		}

		[Theory]
		[NancyInlineAutoData("GET", true)]
		[NancyInlineAutoData("GET", false)]
		[NancyInlineAutoData("DELETE", true)]
		[NancyInlineAutoData("DELETE", false)]
		[NancyInlineAutoData("OPTIONS", true)]
		[NancyInlineAutoData("OPTIONS", false)]
		public void ExpectRoutesForMultipleServicesRegisteredByOverloadedRegistrationMethodAreWiredUpAndResolvedCorrectly(
			string verb,
			bool useParamsRegistration,
			RouteRegistrar registrar,
			[WithinInclusiveRange(-1000, 1000)] int a,
			[WithinInclusiveRange(-1000, 1000)] int b)
		{
			var browser = new Browser(with => with.Module(new CalculatorModule(registrar, useParamsRegistration)));
			browser.SendUrlRequest<CalculatorResponse>(verb, "/add/" + a + "/" + b).Result.Should().Be(a + b, " [add]");
			browser.SendUrlRequest<CalculatorResponse>(verb, "/multiply/" + a + "/" + b).Result.Should().Be(a * b, " [multiply]");
		}
	}
}
