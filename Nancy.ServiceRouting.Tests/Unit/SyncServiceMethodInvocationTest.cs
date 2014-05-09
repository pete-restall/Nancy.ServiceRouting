using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class SyncServiceMethodInvocationTest
	{
		public class StubService
		{
			public virtual Response ServiceMethodWithResponse(Request request) { return new Response(); }
			public virtual void ServiceMethodWithNoResponse(Request request) { }
			public Response MethodWithMoreThanOneParameter(Request request1, Request request2) { return new Response(); }
			public Response MethodWithNoParameters() { return new Response(); }
			public async void MethodDecoratedWithAsync(Request request) { await LongRunningTask.Instance(); }
			public Task MethodReturningTask(Request request) { return LongRunningTask.Instance(); }
			public Task<T> MethodReturningTaskOf<T>(Request request) { return LongRunningTask.Instance<T>(); }
		}

		public class Request { }

		public class Response { }

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new SyncServiceMethodInvocation().Invoking(x => x.CanCreateInvocationDelegateFor(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithMethodHavingMoreThanOneParameter_ExpectFalseIsReturned()
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(
				InfoOf.Method<StubService>(svc => svc.MethodWithMoreThanOneParameter(null, null)))
					.Should().BeFalse();
		}

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithMethodHavingNoParameters_ExpectFalseIsReturned()
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(
				InfoOf.Method<StubService>(svc => svc.MethodWithNoParameters()))
					.Should().BeFalse();
		}

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithAsyncDecoratedMethod_ExpectFalseIsReturned()
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(
				InfoOf.Method<StubService>(svc => svc.MethodDecoratedWithAsync(null)))
					.Should().BeFalse();
		}

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithMethodReturningTask_ExpectFalseIsReturned()
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(
				InfoOf.Method<StubService>(svc => svc.MethodReturningTask(null)))
					.Should().BeFalse();
		}

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithMethodReturningTaskOfT_ExpectFalseIsReturned()
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(
				InfoOf.Method<StubService>(svc => svc.MethodReturningTaskOf<object>(null)))
					.Should().BeFalse();
		}

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithMethodTakingSingleParameterAndReturningNoResponse_ExpectTrueIsReturned()
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(
				InfoOf.Method<StubService>(svc => svc.ServiceMethodWithNoResponse(null)))
					.Should().BeTrue();
		}

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithMethodTakingSingleParameterAndReturningNonTaskResponse_ExpectTrueIsReturned()
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(
				InfoOf.Method<StubService>(svc => svc.ServiceMethodWithResponse(null)))
					.Should().BeTrue();
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceFactory_ExpectArgumentNullExceptionWithCorrectParamName(Response defaultResponse)
		{
			new SyncServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					null,
					DummyRequestBinder,
					InfoOf.Method<StubService>(svc => svc.ServiceMethodWithResponse(null)),
					defaultResponse))
			.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceFactory");
		}

		private static Func<object, object> DummyRequestBinder
		{
			get { return x => new object(); }
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullRequestBinder_ExpectArgumentNullExceptionWithCorrectParamName(Response defaultResponse)
		{
			new SyncServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					DummyServiceFactory,
					null,
					InfoOf.Method<StubService>(svc => svc.ServiceMethodWithResponse(null)),
					defaultResponse))
			.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestBinder");
		}

		private static Func<object> DummyServiceFactory
		{
			get { return () => new object(); }
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName(Response defaultResponse)
		{
			new SyncServiceMethodInvocation().Invoking(x => x.CreateInvocationDelegate(DummyServiceFactory, DummyRequestBinder, null, defaultResponse))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithNullDefaultResponse_ExpectNoArgumentNullException()
		{
			new SyncServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					DummyServiceFactory,
					DummyRequestBinder,
					InfoOf.Method<StubService>(svc => svc.ServiceMethodWithResponse(null)),
					null))
			.ShouldNotThrow<ArgumentNullException>();
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithMethodThatReturnsTask_ExpectArgumentExceptionWithCorrectParamName()
		{
			ExpectCreateInvocationDelegateThrowsArgumentExceptionForMethod(InfoOf.Method<StubService>(svc => svc.MethodReturningTask(null)));
		}

		private static void ExpectCreateInvocationDelegateThrowsArgumentExceptionForMethod(MethodInfo invalidServiceMethod)
		{
			new SyncServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					DummyServiceFactory,
					DummyRequestBinder,
					invalidServiceMethod,
					new object()))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithMethodThatReturnsTaskOfT_ExpectArgumentExceptionWithCorrectParamName()
		{
			ExpectCreateInvocationDelegateThrowsArgumentExceptionForMethod(InfoOf.Method<StubService>(svc => svc.MethodReturningTaskOf<int>(null)));
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithMethodDecoratedWithAsync_ExpectArgumentExceptionWithCorrectParamName()
		{
			ExpectCreateInvocationDelegateThrowsArgumentExceptionForMethod(InfoOf.Method<StubService>(svc => svc.MethodDecoratedWithAsync(null)));
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsResponse_ExpectReturnedDelegateReturnsSameResponseAsServiceMethod(
			object request, Request dto, Response response, Response defaultResponse)
		{
			var service = MockRepository.GenerateStub<StubService>();
			service.Stub(x => x.ServiceMethodWithResponse(Arg<Request>.Is.Same(dto))).Return(response);

			var lambda = (Func<object, object>) new SyncServiceMethodInvocation().CreateInvocationDelegate(
				() => service,
				StubRequestBinderFor(request, dto),
				InfoOf.Method<StubService>(x => x.ServiceMethodWithResponse(null)),
				defaultResponse);

			lambda(request).Should().BeSameAs(response);
		}

		private static Func<object, object> StubRequestBinderFor(object request, Request dto)
		{
			var requestBinder = MockRepository.GenerateStub<Func<object, object>>();
			requestBinder.Stub(x => x(Arg<object>.Is.Same(request))).Return(dto);
			return requestBinder;
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithMethodHavingMoreThanOneParameter_ExpectArgumentExceptionWithCorrectParamName(Response defaultResponse)
		{
			new SyncServiceMethodInvocation().Invoking(x => x.CreateInvocationDelegate(
				DummyServiceFactory, DummyRequestBinder, InfoOf.Method<StubService>(svc => svc.MethodWithMoreThanOneParameter(null, null)), defaultResponse))
					.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithMethodHavingNoParameters_ExpectArgumentExceptionWithCorrectParamName(Response defaultResponse)
		{
			new SyncServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					DummyServiceFactory,
					DummyRequestBinder,
					InfoOf.Method<StubService>(svc => svc.MethodWithNoParameters()),
					defaultResponse))
			.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsNoResponse_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, object request, Request dto, Response defaultResponse)
		{
			var lambda = (Func<object, object>) new SyncServiceMethodInvocation().CreateInvocationDelegate(
				() => service,
				StubRequestBinderFor(request, dto),
				InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)),
				defaultResponse);

			lambda(request).Should().BeSameAs(defaultResponse);
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsNoResponse_ExpectServiceMethodIsStillCalled(
			object request, Request dto, Response defaultResponse)
		{
			var service = MockRepository.GenerateMock<StubService>();
			var lambda = (Func<object, object>) new SyncServiceMethodInvocation().CreateInvocationDelegate(
				() => service,
				StubRequestBinderFor(request, dto),
				InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)),
				defaultResponse);

			lambda(request);
			service.AssertWasCalled(x => x.ServiceMethodWithNoResponse(Arg<Request>.Is.Same(dto)), x => x.Repeat.Once());
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithRequestOfIncompatibleType_ExpectExceptionRatherThanPassingNullRequestToService(
			object incompatibleRequest, Response defaultResponse)
		{
			var service = MockRepository.GenerateStrictMock<StubService>();
			var lambda = (Func<object, object>) new SyncServiceMethodInvocation().CreateInvocationDelegate(
				() => service, x => x, InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)), defaultResponse);

			lambda.Invoking(x => x(incompatibleRequest)).ShouldThrow<InvalidCastException>();
		}
	}
}
