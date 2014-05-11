using System;
using System.Threading;
using FluentAssertions;
using Nancy.Testing;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Restall.Nancy.ServiceRouting.Tests.Integration.AsyncCancelService;
using Restall.Nancy.ServiceRouting.Tests.Integration.CalculatorService;
using Restall.Nancy.ServiceRouting.Tests.Integration.EchoService;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public class AsyncRouteFormRequestTest
	{
		private const int AsyncSafetyTimeoutSeconds = 10;

		private const int AsyncSafetyTimeoutMilliseconds = AsyncSafetyTimeoutSeconds * 1000;

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds)]
		[NancyInlineAutoData("PUT")]
		[NancyInlineAutoData("POST")]
		[NancyInlineAutoData("PATCH")]
		public void ExpectRouteIsWiredUpAndResolvedCorrectly(string verb, RouteRegistrar registrar, EchoFormRequest request)
		{
			var browser = new Browser(with => with.Module(new AsyncEchoModule(registrar)));
			browser.SendFormRequest<EchoResponse>(verb, "/echo", request).TokenEcho.Should().Be(request.Token);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds)]
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
			var browser = new Browser(with => with.Module(new AsyncCalculatorModule(registrar, useParamsRegistration)));
			browser.SendFormRequest<CalculatorResponse>(verb, "/add", new AddRequest(a, b)).Result.Should().Be(a + b, " [add]");
			browser.SendFormRequest<CalculatorResponse>(verb, "/multiply", new MultiplyRequest(a, b)).Result.Should().Be(a * b, " [multiply]");
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds)]
		[NancyInlineAutoData("PUT")]
		[NancyInlineAutoData("POST")]
		[NancyInlineAutoData("PATCH")]
		public void ExpectCancellableRouteIsWiredUpAndResolvedCorrectly(
			string verb, RouteRegistrar registrar, Guid token, CancellationTokenSource cancel)
		{
			var browser = AsyncCancellationBrowserFactory.CreateBrowserWithCancellationToken(registrar, cancel.Token);
			var request = new LongRunningFormRequest(AsyncSafetyTimeoutSeconds + 1, token);

			cancel.CancelAfter(TimeSpan.FromMilliseconds(20));
			var response = browser.SendFormRequest<CancelResponse>(verb, "/cancel/long-running", request);

			response.TokenEcho.Should().Be(token);
			response.WasCancelled.Should().BeTrue();
		}
	}
}
