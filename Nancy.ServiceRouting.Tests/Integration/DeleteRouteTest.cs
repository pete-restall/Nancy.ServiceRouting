﻿using System;
using FluentAssertions;
using Nancy.ServiceRouting.Tests.AutoFixture;
using Nancy.ServiceRouting.Tests.Integration.CalculatorService;
using Nancy.ServiceRouting.Tests.Integration.EchoService;
using Nancy.Testing;
using Xunit.Extensions;

namespace Nancy.ServiceRouting.Tests.Integration
{
	public class DeleteRouteTest
	{
		[Theory, NancyAutoData]
		public void ExpectDeleteRouteIsWiredUpAndResolvedCorrectly(RouteRegistrar registrar, Guid token)
		{
			var browser = new Browser(with => with.Module(new EchoModule(registrar)));
			browser.Delete<EchoResponse>("/echo/" + token).TokenEcho.Should().Be(token);
		}

		[Theory]
		[NancyInlineAutoData(true)]
		[NancyInlineAutoData(false)]
		public void ExpectDeleteRoutesForMultipleServicesRegisteredByOverloadedRegistrationMethodAreWiredUpAndResolvedCorrectly(
			bool useParamsRegistration, RouteRegistrar registrar, [WithinInclusiveRange(-1000, 1000)] int a, [WithinInclusiveRange(-1000, 1000)] int b)
		{
			var browser = new Browser(with => with.Module(new CalculatorModule(registrar, useParamsRegistration)));
			browser.Delete<CalculatorResponse>("/add/" + a + "/" + b).Result.Should().Be(a + b, " [add]");
			browser.Delete<CalculatorResponse>("/multiply/" + a + "/" + b).Result.Should().Be(a * b, " [multiply]");
		}
	}
}
