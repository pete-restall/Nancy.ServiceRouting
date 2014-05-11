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

	public class AsyncVoidServiceMethodInvocationTest
	{
		public class StubService
		{
			public async virtual void AsyncDecoratedMethodWithSingleParameter(Request request)
				{ await LongRunningTask.Instance<Response>(); }

			public async virtual void AsyncDecoratedMethodWithCancellationToken(Request request, CancellationToken cancel)
				{ await LongRunningTask.Instance<Response>(); }
		}

		private class StubServiceUnhandled
		{
			public async virtual Task<Response> AsyncDecoratedMethodWithTaskOfTResponse(Request request)
				{ return await LongRunningTask.Instance<Response>(); }

			public async virtual Task AsyncDecoratedMethodWithTaskResponse(Request request)
				{ await LongRunningTask.Instance(); }

			public virtual void NonAsyncDecoratedMethod(Request request) { }

			public virtual void NonAsyncDecoratedMethodWithCancellationToken(Request request, CancellationToken cancel) { }

			public virtual Task<Response> NonAsyncDecoratedMethodWithTaskOfTResponse(Request request)
				{ return LongRunningTask.Instance<Response>(); }

			public virtual Task NonAsyncDecoratedMethodWithTaskResponse(Request request) { return LongRunningTask.Instance(); }

			public async virtual void AsyncDecoratedMethodWithNoParameters() { await LongRunningTask.Instance<Response>(); }

			public async virtual void AsyncDecoratedMethodWithTwoParameters(Request request, object tooMany)
				{ await LongRunningTask.Instance(); }

			public async virtual Task<Response> AsyncDecoratedMethodWithCancellationTokenAndTaskOfTResponse(Request request, CancellationToken cancel)
				{ return await LongRunningTask.Instance<Response>(); }

			public async virtual Task AsyncDecoratedMethodWithCancellationTokenAndTaskResponse(Request request, CancellationToken cancel)
				{ await LongRunningTask.Instance(); }

			public static async void AsyncDecoratedStaticMethod(Request request) { await LongRunningTask.Instance<Response>(); }

			public async virtual void AsyncDecoratedMethodWithCancellationTokenAndOtherParameters(Request request, CancellationToken cancel, object other)
				{ await LongRunningTask.Instance(); }

			public async virtual void AsyncDecoratedMethodWithCancellationTokenInWrongPlace(CancellationToken cancel, Request request)
				{ await LongRunningTask.Instance(); }
		}

		public class Request { }

		public class Response { }

		private const int AsyncSafetyTimeoutSeconds = 30;

		private const int AsyncSafetyTimeoutMilliseconds = AsyncSafetyTimeoutSeconds * 1000;

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new AsyncVoidServiceMethodInvocation().Invoking(x => x.CanCreateInvocationDelegateFor(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyInlineAutoData(typeof(StubServiceUnhandled))]
		public void CanCreateInvocationDelegateFor_CalledForAnyMethodInThisType_ExpectFalseIsReturned(Type serviceType)
		{
			serviceType.GetAllDeclaredMethods().ForEach(ExpectCanNotCreateInvocationDelegateFor);
		}

		private static void ExpectCanNotCreateInvocationDelegateFor(MethodInfo method)
		{
			new AsyncVoidServiceMethodInvocation().CanCreateInvocationDelegateFor(method)
				.Should().BeFalse(method + " is not of the correct signature");
		}

		[Theory, NancyInlineAutoData(typeof(StubService))]
		public void CanCreateInvocationDelegateFor_CalledForAnyMethodInThisType_ExpectTrueIsReturned(Type serviceType)
		{
			serviceType.GetAllDeclaredMethods().ForEach(ExpectCanCreateInvocationDelegateFor);
		}

		private static void ExpectCanCreateInvocationDelegateFor(MethodInfo method)
		{
			new AsyncVoidServiceMethodInvocation().CanCreateInvocationDelegateFor(method)
				.Should().BeTrue(method + " is of the correct signature");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName(
			ServiceMethodInvocationContext context)
		{
			new AsyncVoidServiceMethodInvocation().Invoking(x => x.CreateInvocationDelegate(null, context))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithNullContext_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new AsyncVoidServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithSingleParameter(null)),
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
			new AsyncVoidServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(invalidServiceMethod, context))
					.ShouldThrow<ArgumentException>()
					.And.ParamName.Should().Be("serviceMethod", invalidServiceMethod + " is not of the correct signature");
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithSingleParameter_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, object request, Request dto, Response defaultResponse)
		{
			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncVoidServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithSingleParameter(null)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Result.Should().BeSameAs(defaultResponse);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithSingleParameter_ExpectServiceMethodIsStillCalled(
			object request, Request dto, Response defaultResponse)
		{
			var wasCalledWithCorrectArguments = new ManualResetEvent(false);
			var service = MockRepository.GenerateStrictMock<StubService>();
			service.Expect(x => x.AsyncDecoratedMethodWithSingleParameter(Arg<Request>.Is.Same(dto)))
				.Do(new Action<Request>(x => wasCalledWithCorrectArguments.Set()));

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncVoidServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithSingleParameter(null)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Wait();
			wasCalledWithCorrectArguments.WaitOne().Should().BeTrue();
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithCancellationToken_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, object request, Request dto, Response defaultResponse)
		{
			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncVoidServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithCancellationToken(null, CancellationToken.None)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Result.Should().BeSameAs(defaultResponse);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithCancellationToken_ExpectServiceMethodIsStillCalled(
			object request, Request dto, Response defaultResponse)
		{
			var wasCalledWithCorrectArguments = new ManualResetEvent(false);
			var service = MockRepository.GenerateStrictMock<StubService>();
			service.Expect(x => x.AsyncDecoratedMethodWithCancellationToken(Arg<Request>.Is.Same(dto), Arg<CancellationToken>.Is.Anything))
				.Do(new Action<Request, CancellationToken>((x, c) =>
					{
						c.WaitHandle.WaitOne(TimeSpan.FromSeconds(AsyncSafetyTimeoutSeconds + 1));
						wasCalledWithCorrectArguments.Set();
					}));

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncVoidServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithCancellationToken(null, CancellationToken.None)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));
			lambda(request, cancel.Token).Wait();
			wasCalledWithCorrectArguments.WaitOne().Should().BeTrue();
		}
	}

	#pragma warning restore 4014
}
