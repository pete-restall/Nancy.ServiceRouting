using FluentAssertions;
using Nancy.ServiceRouting.Tests.AutoFixture;
using Nancy.ServiceRouting.Tests.Integration.CalculatorService;
using Nancy.ServiceRouting.Tests.Integration.EchoService;
using Nancy.Testing;
using Xunit.Extensions;

namespace Nancy.ServiceRouting.Tests.Integration
{
	public class PatchRouteTest
	{
		[Theory, NancyAutoData]
		public void ExpectPatchRouteIsWiredUpAndResolvedCorrectly(RouteRegistrar registrar, EchoFormRequest request)
		{
			var browser = new Browser(with => with.Module(new EchoModule(registrar)));
			browser.Patch<EchoResponse>("/echo", request).TokenEcho.Should().Be(request.Token);
		}

		[Theory]
		[NancyInlineAutoData(true)]
		[NancyInlineAutoData(false)]
		public void ExpectPatchRoutesForMultipleServicesRegisteredByOverloadedRegistrationMethodAreWiredUpAndResolvedCorrectly(
			bool useParamsRegistration, RouteRegistrar registrar, [WithinInclusiveRange(-1000, 1000)] int a, [WithinInclusiveRange(-1000, 1000)] int b)
		{
			var browser = new Browser(with => with.Module(new CalculatorModule(registrar, useParamsRegistration)));
			browser.Patch<CalculatorResponse>("/add", new AddRequest(a, b)).Result.Should().Be(a + b, " [add]");
			browser.Patch<CalculatorResponse>("/multiply", new MultiplyRequest(a, b)).Result.Should().Be(a * b, " [multiply]");
		}
	}
}
