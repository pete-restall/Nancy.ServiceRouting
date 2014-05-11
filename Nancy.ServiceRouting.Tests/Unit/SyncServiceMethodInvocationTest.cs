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
		}

		private class StubServiceUnhandled
		{
			public virtual Response MethodWithMoreThanOneParameter(Request request1, Request request2) { return new Response(); }
			public virtual Response MethodWithNoParameters() { return new Response(); }
			public virtual async void MethodDecoratedWithAsync(Request request) { await LongRunningTask.Instance(); }
			public virtual Task MethodReturningTask(Request request) { return LongRunningTask.Instance(); }
			public virtual Task<T> MethodReturningTaskOf<T>(Request request) { return LongRunningTask.Instance<T>(); }
			public static Response StaticMethod(Request request) { return new Response(); }
		}

		public class Request { }

		public class Response { }

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new SyncServiceMethodInvocation().Invoking(x => x.CanCreateInvocationDelegateFor(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyInlineAutoData(typeof(StubServiceUnhandled))]
		public void CanCreateInvocationDelegateFor_CalledForAnyMethodInThisType_ExpectFalseIsReturned(Type serviceType)
		{
			serviceType.GetAllDeclaredMethods().ForEach(ExpectCanNotCreateInvocationDelegateFor);
		}

		private static void ExpectCanNotCreateInvocationDelegateFor(MethodInfo method)
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(method)
				.Should().BeFalse(method + " is not of the correct signature");
		}

		[Theory, NancyInlineAutoData(typeof(StubService))]
		public void CanCreateInvocationDelegateFor_CalledForAnyMethodInThisType_ExpectTrueIsReturned(Type serviceType)
		{
			serviceType.GetAllDeclaredMethods().ForEach(ExpectCanCreateInvocationDelegateFor);
		}

		private static void ExpectCanCreateInvocationDelegateFor(MethodInfo method)
		{
			new SyncServiceMethodInvocation().CanCreateInvocationDelegateFor(method)
				.Should().BeTrue(method + " is of the correct signature");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName(
			ServiceMethodInvocationContext context)
		{
			new SyncServiceMethodInvocation().Invoking(x => x.CreateInvocationDelegate(null, context))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithNullContext_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new SyncServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					InfoOf.Method<StubService>(svc => svc.ServiceMethodWithResponse(null)),
					null))
			.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("context");
		}

		[Theory, NancyInlineAutoData(typeof(StubServiceUnhandled))]
		public void CreateInvocationDelegate_CalledForAnyMethodInThisType_ExpectArgumentExceptionWithCorrectParamName(
			Type serviceType, ServiceMethodInvocationContext context)
		{
			serviceType.GetAllDeclaredMethods().ForEach(m => ExpectCreateInvocationDelegateThrowsArgumentExceptionForMethod(m, context));
		}

		private static void ExpectCreateInvocationDelegateThrowsArgumentExceptionForMethod(
			MethodInfo invalidServiceMethod, ServiceMethodInvocationContext context)
		{
			new SyncServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(invalidServiceMethod, context))
					.ShouldThrow<ArgumentException>()
					.And.ParamName.Should().Be("serviceMethod", invalidServiceMethod + " is not of the correct signature");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsResponse_ExpectReturnedDelegateReturnsSameResponseAsServiceMethod(
			object request, Request dto, Response response)
		{
			var service = MockRepository.GenerateStub<StubService>();
			service.Stub(x => x.ServiceMethodWithResponse(Arg<Request>.Is.Same(dto))).Return(response);

			var lambda = (Func<object, object>) new SyncServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(x => x.ServiceMethodWithResponse(null)), Stub.InvocationContextFor(service, request, dto));

			lambda(request).Should().BeSameAs(response);
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsNoResponse_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, object request, Request dto, Response defaultResponse)
		{
			var lambda = (Func<object, object>) new SyncServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request).Should().BeSameAs(defaultResponse);
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithServiceMethodThatReturnsNoResponse_ExpectServiceMethodIsStillCalled(
			object request, Request dto, Response defaultResponse)
		{
			var service = MockRepository.GenerateMock<StubService>();
			var lambda = (Func<object, object>) new SyncServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request);
			service.AssertWasCalled(x => x.ServiceMethodWithNoResponse(Arg<Request>.Is.Same(dto)), x => x.Repeat.Once());
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithRequestOfIncompatibleType_ExpectExceptionRatherThanPassingNullRequestToService(
			object incompatibleRequest)
		{
			var service = MockRepository.GenerateStrictMock<StubService>();
			var lambda = (Func<object, object>) new SyncServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(x => x.ServiceMethodWithNoResponse(null)),
				Stub.InvocationContextFor(service, incompatibleRequest, incompatibleRequest));

			lambda.Invoking(x => x(incompatibleRequest)).ShouldThrow<InvalidCastException>();
		}
	}
}
