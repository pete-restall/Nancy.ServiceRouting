using System;
using FluentAssertions;
using Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Nancy.ServiceRouting.Tests.Unit
{
	public class DefaultServiceMethodInvocationTest
	{
		public class StubService
		{
			public virtual Response ServiceMethodWithResponse(Request request) { return new Response(); }
			public virtual void ServiceMethodWithNoResponse(Request request) { }
			public Response MethodWithMoreThanOneParameter(Request request1, Request request2) { return new Response(); }
			public Response MethodWithNoParameters() { return new Response(); }
		}

		public class Request { }

		public class Response { }

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceFactory_ExpectArgumentNullExceptionWithCorrectParamName(Response defaultResponse)
		{
			new DefaultServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(null, InfoOf.Method<StubService>(svc => svc.ServiceMethodWithResponse(null)), defaultResponse))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceFactory");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName(Response defaultResponse)
		{
			new DefaultServiceMethodInvocation().Invoking(x => x.CreateInvocationDelegate(DummyServiceFactory, null, defaultResponse))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		private static Func<object> DummyServiceFactory
		{
			get { return () => new object(); }
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithNullDefaultResponse_ExpectNoArgumentNullException()
		{
			new DefaultServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(DummyServiceFactory, InfoOf.Method<StubService>(svc => svc.ServiceMethodWithResponse(null)), null))
					.ShouldNotThrow<ArgumentNullException>();
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsResponse_ExpectReturnedDelegateReturnsSameResponseAsServiceMethod(
			Request request, Response response, Response defaultResponse)
		{
			var service = MockRepository.GenerateStub<StubService>();
			service.Stub(x => x.ServiceMethodWithResponse(Arg<Request>.Is.Same(request))).Return(response);

			var lambda = new DefaultServiceMethodInvocation().CreateInvocationDelegate(
				() => service, InfoOf.Method<StubService>(x => x.ServiceMethodWithResponse(null)), defaultResponse);

			lambda(request).Should().BeSameAs(response);
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithMethodHavingMoreThanOneParameter_ExpectArgumentExceptionWithCorrectParamName(Response defaultResponse)
		{
			new DefaultServiceMethodInvocation().Invoking(x => x.CreateInvocationDelegate(
				DummyServiceFactory, InfoOf.Method<StubService>(svc => svc.MethodWithMoreThanOneParameter(null, null)), defaultResponse))
					.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithMethodHavingNoParameters_ExpectArgumentExceptionWithCorrectParamName(Response defaultResponse)
		{
			new DefaultServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(DummyServiceFactory, InfoOf.Method<StubService>(svc => svc.MethodWithNoParameters()), defaultResponse))
					.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsNoResponse_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, Request request, Response defaultResponse)
		{
			var lambda = new DefaultServiceMethodInvocation().CreateInvocationDelegate(
				() => service, InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)), defaultResponse);

			lambda(request).Should().BeSameAs(defaultResponse);
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsNoResponse_ExpectServiceMethodIsStillCalled(
			Request request, Response defaultResponse)
		{
			var service = MockRepository.GenerateMock<StubService>();
			var lambda = new DefaultServiceMethodInvocation().CreateInvocationDelegate(
				() => service, InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)), defaultResponse);

			lambda(request);
			service.AssertWasCalled(x => x.ServiceMethodWithNoResponse(Arg<Request>.Is.Same(request)), x => x.Repeat.Once());
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithRequestOfIncompatibleType_ExpectExceptionRatherThanPassingNullRequestToService(
			Request request, object incompatibleRequest, Response defaultResponse)
		{
			var service = MockRepository.GenerateStrictMock<StubService>();
			var lambda = new DefaultServiceMethodInvocation().CreateInvocationDelegate(
				() => service, InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)), defaultResponse);

			lambda.Invoking(x => x(incompatibleRequest)).ShouldThrow<InvalidCastException>();
		}
	}
}
