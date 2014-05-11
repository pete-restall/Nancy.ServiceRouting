using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	#pragma warning disable 4014

	public class AsyncTaskOfTServiceMethodInvocationTest
	{
		public class StubService
		{
			public async virtual Task<Response> AsyncDecoratedMethodWithTaskOfTResponse(Request request)
				{ return await LongRunningTask.Instance<Response>(); }

			public virtual Task<Response> NonAsyncDecoratedMethodWithTaskOfTResponse(Request request)
				{ return LongRunningTask.Instance<Response>(); }

			public async virtual Task<Response> AsyncDecoratedMethodWithCancellationTokenAndTaskOfTResponse(Request request, CancellationToken cancel)
				{ return await LongRunningTask.Instance<Response>(); }

			public virtual Task<Response> NonAsyncDecoratedMethodWithCancellationTokenAndTaskOfTResponse(Request request, CancellationToken cancel)
				{ return LongRunningTask.Instance<Response>(); }
		}

		private class StubServiceUnhandled
		{
			public async virtual Task AsyncDecoratedMethodWithTaskResponse(Request request)
				{ await LongRunningTask.Instance(); }

			public async virtual void AsyncDecoratedMethodWithNoResponse(Request request)
				{ await LongRunningTask.Instance(); }

			public virtual Task NonAsyncDecoratedMethodWithTaskResponse(Request request)
				{ return LongRunningTask.Instance(); }

			public virtual void NonAsyncDecoratedMethodWithNoResponse(Request request) { }

			public async virtual Task<Response> AsyncDecoratedMethodWithNoParameters()
				{ return await LongRunningTask.Instance<Response>(); }

			public virtual Task<Response> NonAsyncDecoratedMethodWithTwoParameters(Request request, object tooMany)
				{ return LongRunningTask.Instance<Response>(); }

			public virtual Task<Response> NonAsyncDecoratedMethodWithNoParameters()
				{ return LongRunningTask.Instance<Response>(); }

			public async virtual Task<Response> AsyncDecoratedMethodWithTwoParameters(Request request, object tooMany)
				{ return await LongRunningTask.Instance<Response>(); }

			public async virtual Task AsyncDecoratedMethodWithTaskResponseAndCancellationToken(Request request, CancellationToken cancel)
				{ await LongRunningTask.Instance(); }

			public static async Task<Response> StaticMethod(Request request)
				{ return await LongRunningTask.Instance<Response>(); }

			public async virtual Task<Response> AsyncDecoratedMethodWithCancellationTokenAndOtherParameters(
				Request request, CancellationToken cancel, object other)
					{ return await LongRunningTask.Instance<Response>(); }

			public async virtual Task<Response> AsyncDecoratedMethodWithCancellationTokenInWrongPlace(CancellationToken cancel, Request request)
				{ return await LongRunningTask.Instance<Response>(); }

			public virtual Task<Response> NonAsyncDecoratedMethodWithCancellationTokenAndOtherParameters(
				Request request, CancellationToken cancel, object other)
					{ return LongRunningTask.Instance<Response>(); }

			public virtual Task<Response> NonAsyncDecoratedMethodWithCancellationTokenInWrongPlace(CancellationToken cancel, Request request)
				{ return LongRunningTask.Instance<Response>(); }
		}

		public class Request { }

		public class Response { }

		private const int AsyncSafetyTimeoutSeconds = 30;

		private const int AsyncSafetyTimeoutMilliseconds = AsyncSafetyTimeoutSeconds * 1000;

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new AsyncTaskOfTServiceMethodInvocation().Invoking(x => x.CanCreateInvocationDelegateFor(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyInlineAutoData(typeof(StubServiceUnhandled))]
		public void CanCreateInvocationDelegateFor_CalledForAnyMethodInThisType_ExpectFalseIsReturned(Type serviceType)
		{
			serviceType.GetAllDeclaredMethods().ForEach(ExpectCanNotCreateInvocationDelegateFor);
		}

		private static void ExpectCanNotCreateInvocationDelegateFor(MethodInfo method)
		{
			new AsyncTaskOfTServiceMethodInvocation().CanCreateInvocationDelegateFor(method)
				.Should().BeFalse(method + " is not of the correct signature");
		}

		[Theory, NancyInlineAutoData(typeof(StubService))]
		public void CanCreateInvocationDelegateFor_CalledForAnyMethodInThisType_ExpectTrueIsReturned(Type serviceType)
		{
			serviceType.GetAllDeclaredMethods().ForEach(ExpectCanCreateInvocationDelegateFor);
		}

		private static void ExpectCanCreateInvocationDelegateFor(MethodInfo method)
		{
			new AsyncTaskOfTServiceMethodInvocation().CanCreateInvocationDelegateFor(method)
				.Should().BeTrue(method + " is of the correct signature");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName(
			ServiceMethodInvocationContext context)
		{
			new AsyncTaskOfTServiceMethodInvocation().Invoking(x => x.CreateInvocationDelegate(null, context))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithNullContext_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new AsyncTaskOfTServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithTaskOfTResponse(null)),
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
			new AsyncTaskOfTServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(invalidServiceMethod, context))
					.ShouldThrow<ArgumentException>()
					.And.ParamName.Should().Be("serviceMethod", invalidServiceMethod + " is not of the correct signature");
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithSingleParameter_ExpectReturnedDelegateReturnsResponse(
			object request, Request dto, Response response)
		{
			var service = MockRepository.GenerateStub<StubService>();
			service.Stub(x => x.AsyncDecoratedMethodWithTaskOfTResponse(Arg<Request>.Is.Same(dto))).Return(Task.FromResult(response));

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskOfTServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithTaskOfTResponse(null)),
				Stub.InvocationContextFor(service, request, dto));

			lambda(request, CancellationToken.None).Result.Should().BeSameAs(response);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNonAsyncDecoratedMethodWithSingleParameter_ExpectReturnedDelegateReturnsResponse(
			object request, Request dto, Response response)
		{
			var service = MockRepository.GenerateStub<StubService>();
			service.Stub(x => x.NonAsyncDecoratedMethodWithTaskOfTResponse(Arg<Request>.Is.Same(dto))).Return(Task.FromResult(response));

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskOfTServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.NonAsyncDecoratedMethodWithTaskOfTResponse(null)),
				Stub.InvocationContextFor(service, request, dto));

			lambda(request, CancellationToken.None).Result.Should().BeSameAs(response);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithCancellationToken_ExpectReturnedDelegateReturnsResponse(
			object request, Request dto, Response response)
		{
			var service = CreateStubServiceToReturnAfterCancellation(
				x => x.AsyncDecoratedMethodWithCancellationTokenAndTaskOfTResponse(Arg<Request>.Is.Same(dto), Arg<CancellationToken>.Is.Anything),
				response);

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskOfTServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithCancellationTokenAndTaskOfTResponse(null, CancellationToken.None)),
				Stub.InvocationContextFor(service, request, dto));

			var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));
			lambda(request, cancel.Token).Result.Should().BeSameAs(response);
		}

		private static StubService CreateStubServiceToReturnAfterCancellation(Function<StubService, Task<Response>> expectation, Response response)
		{
			var service = MockRepository.GenerateStub<StubService>();
			service.Stub(expectation).Do(new Func<Request, CancellationToken, Task<Response>>((r, c) =>
				{
					c.WaitHandle.WaitOne();
					return Task.FromResult(response);
				}));

			return service;
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNonAsyncDecoratedMethodWithCancellationToken_ExpectReturnedDelegateReturnsResponse(
			object request, Request dto, Response response)
		{
			var service = CreateStubServiceToReturnAfterCancellation(
				x => x.NonAsyncDecoratedMethodWithCancellationTokenAndTaskOfTResponse(Arg<Request>.Is.Same(dto), Arg<CancellationToken>.Is.Anything),
				response);

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskOfTServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.NonAsyncDecoratedMethodWithCancellationTokenAndTaskOfTResponse(null, CancellationToken.None)),
				Stub.InvocationContextFor(service, request, dto));

			var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));
			lambda(request, cancel.Token).Result.Should().BeSameAs(response);
		}
	}

	#pragma warning restore 4014
}
