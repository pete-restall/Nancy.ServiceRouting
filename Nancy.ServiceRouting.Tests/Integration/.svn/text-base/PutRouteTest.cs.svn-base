using FluentAssertions;
using Nancy.ServiceRouting.Tests.AutoFixture;
using Nancy.ServiceRouting.Tests.Integration.CalculatorService;
using Nancy.ServiceRouting.Tests.Integration.EchoService;
using Nancy.Testing;
using Xunit.Extensions;

namespace Nancy.ServiceRouting.Tests.Integration
{
	public class PutRouteTest
	{
		[Theory, NancyAutoData]
		public void ExpectPutRouteIsWiredUpAndResolvedCorrectly(RouteRegistrar registrar, EchoFormRequest request)
		{
			var browser = new Browser(with => with.Module(new EchoModule(registrar)));
			browser.Put<EchoResponse>("/echo", request).TokenEcho.Should().Be(request.Token);
		}

		[Theory]
		[NancyInlineAutoData(true)]
		[NancyInlineAutoData(false)]
		public void ExpectPutRoutesForMultipleServicesRegisteredByOverloadedRegistrationMethodAreWiredUpAndResolvedCorrectly(
			bool useParamsRegistration, RouteRegistrar registrar, [WithinInclusiveRange(-1000, 1000)] int a, [WithinInclusiveRange(-1000, 1000)] int b)
		{
			var browser = new Browser(with => with.Module(new CalculatorModule(registrar, useParamsRegistration)));
			browser.Put<CalculatorResponse>("/add", new AddRequest(a, b)).Result.Should().Be(a + b, " [add]");
			browser.Put<CalculatorResponse>("/multiply", new MultiplyRequest(a, b)).Result.Should().Be(a * b, " [multiply]");
		}
	}
}
