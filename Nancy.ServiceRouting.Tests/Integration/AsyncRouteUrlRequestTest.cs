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
	public class AsyncRouteUrlRequestTest
	{
		private const int AsyncSafetyTimeoutSeconds = 30;

		private const int AsyncSafetyTimeoutMilliseconds = AsyncSafetyTimeoutSeconds * 1000;

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds)]
		[NancyInlineAutoData("GET")]
		[NancyInlineAutoData("DELETE")]
		[NancyInlineAutoData("OPTIONS")]
		public void ExpectRouteIsWiredUpAndResolvedCorrectly(string verb, RouteRegistrar registrar, Guid token)
		{
			var browser = new Browser(with => with.Module(new AsyncEchoModule(registrar)));
			browser.SendUrlRequest<EchoResponse>(verb, "/echo/" + token).TokenEcho.Should().Be(token);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds)]
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
			var browser = new Browser(with => with.Module(new AsyncCalculatorModule(registrar, useParamsRegistration)));
			browser.SendUrlRequest<CalculatorResponse>(verb, "/add/" + a + "/" + b).Result.Should().Be(a + b, " [add]");
			browser.SendUrlRequest<CalculatorResponse>(verb, "/multiply/" + a + "/" + b).Result.Should().Be(a * b, " [multiply]");
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds)]
		[NancyInlineAutoData("GET")]
		[NancyInlineAutoData("DELETE")]
		[NancyInlineAutoData("OPTIONS")]
		public void ExpectCancellableRouteIsWiredUpAndResolvedCorrectly(
			string verb, RouteRegistrar registrar, Guid token, CancellationTokenSource cancel)
		{
			var browser = AsyncCancellationBrowserFactory.CreateBrowserWithCancellationToken(registrar, cancel.Token);

			cancel.CancelAfter(TimeSpan.FromMilliseconds(20));
			var response = browser.SendUrlRequest<CancelResponse>(
				verb,
				"/cancel/long-running/" + (AsyncSafetyTimeoutSeconds + 1) + "/" + token);

			response.TokenEcho.Should().Be(token);
			response.WasCancelled.Should().BeTrue();
		}
	}
}
