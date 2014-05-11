using FluentAssertions;
using Nancy.Testing;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Restall.Nancy.ServiceRouting.Tests.Integration.CalculatorService;
using Restall.Nancy.ServiceRouting.Tests.Integration.EchoService;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public class RouteFormRequestTest
	{
		[Theory]
		[NancyInlineAutoData("PUT")]
		[NancyInlineAutoData("POST")]
		[NancyInlineAutoData("PATCH")]
		public void ExpectRouteIsWiredUpAndResolvedCorrectly(string verb, RouteRegistrar registrar, EchoFormRequest request)
		{
			var browser = new Browser(with => with.Module(new EchoModule(registrar)));
			browser.SendFormRequest<EchoResponse>(verb, "/echo", request).TokenEcho.Should().Be(request.Token);
		}

		[Theory]
		[NancyInlineAutoData("PUT", true)]
		[NancyInlineAutoData("PUT", false)]
		[NancyInlineAutoData("POST", true)]
		[NancyInlineAutoData("POST", false)]
		[NancyInlineAutoData("PATCH", true)]
		[NancyInlineAutoData("PATCH", false)]
		public void ExpectRoutesForMultipleServicesRegisteredByOverloadedRegistrationMethodAreWiredUpAndResolvedCorrectly(
			string verb,
			bool useParamsRegistration,
			RouteRegistrar registrar,
			[WithinInclusiveRange(-1000, 1000)] int a,
			[WithinInclusiveRange(-1000, 1000)] int b)
		{
			var browser = new Browser(with => with.Module(new CalculatorModule(registrar, useParamsRegistration)));
			browser.SendFormRequest<CalculatorResponse>(verb, "/add", new AddRequest(a, b)).Result.Should().Be(a + b, " [add]");
			browser.SendFormRequest<CalculatorResponse>(verb, "/multiply", new MultiplyRequest(a, b)).Result.Should().Be(a * b, " [multiply]");
		}
	}
}
