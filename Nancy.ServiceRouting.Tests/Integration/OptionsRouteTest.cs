using System;
using FluentAssertions;
using Nancy.Testing;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Restall.Nancy.ServiceRouting.Tests.Integration.CalculatorService;
using Restall.Nancy.ServiceRouting.Tests.Integration.EchoService;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public class OptionsRouteTest
	{
		[Theory, NancyAutoData]
		public void ExpectOptionsRouteIsWiredUpAndResolvedCorrectly(RouteRegistrar registrar, Guid token)
		{
			var browser = new Browser(with => with.Module(new EchoModule(registrar)));
			browser.Options<EchoResponse>("/echo/" + token).TokenEcho.Should().Be(token);
		}

		[Theory]
		[NancyInlineAutoData(true)]
		[NancyInlineAutoData(false)]
		public void ExpectOptionsRoutesForMultipleServicesRegisteredByOverloadedRegistrationMethodAreWiredUpAndResolvedCorrectly(
			bool useParamsRegistration, RouteRegistrar registrar, [WithinInclusiveRange(-1000, 1000)] int a, [WithinInclusiveRange(-1000, 1000)] int b)
		{
			var browser = new Browser(with => with.Module(new CalculatorModule(registrar, useParamsRegistration)));
			browser.Options<CalculatorResponse>("/add/" + a + "/" + b).Result.Should().Be(a + b, " [add]");
			browser.Options<CalculatorResponse>("/multiply/" + a + "/" + b).Result.Should().Be(a * b, " [multiply]");
		}
	}
}
