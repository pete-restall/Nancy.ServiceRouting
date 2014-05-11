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

	public class AsyncTaskServiceMethodInvocationTest
	{
		public class StubService
		{
			public async virtual Task AsyncDecoratedMethodWithTaskResponse(Request request)
				{ await LongRunningTask.Instance(); }

			public virtual Task NonAsyncDecoratedMethodWithTaskResponse(Request request)
				{ return LongRunningTask.Instance(); }

			public async virtual Task AsyncDecoratedMethodWithCancellationTokenAndTaskResponse(Request request, CancellationToken cancel)
				{ await LongRunningTask.Instance(); }

			public virtual Task NonAsyncDecoratedMethodWithCancellationTokenAndTaskResponse(Request request, CancellationToken cancel)
				{ return LongRunningTask.Instance(); }
		}

		private class StubServiceUnhandled
		{
			public async virtual Task<Response> AsyncDecoratedMethodWithTaskOfTResponse(Request request)
				{ return await LongRunningTask.Instance<Response>(); }

			public async virtual void AsyncDecoratedMethodWithNoResponse(Request request)
				{ await LongRunningTask.Instance(); }

			public virtual Task<Response> NonAsyncDecoratedMethodWithTaskOfTResponse(Request request)
				{ return LongRunningTask.Instance<Response>(); }

			public virtual void NonAsyncDecoratedMethodWithNoResponse(Request request) { }

			public async virtual Task AsyncDecoratedMethodWithNoParameters()
				{ await LongRunningTask.Instance(); }

			public virtual Task NonAsyncDecoratedMethodWithTwoParameters(Request request, object tooMany)
				{ return LongRunningTask.Instance(); }

			public virtual Task NonAsyncDecoratedMethodWithNoParameters()
				{ return LongRunningTask.Instance(); }

			public async virtual Task AsyncDecoratedMethodWithTwoParameters(Request request, object tooMany)
				{ await LongRunningTask.Instance(); }

			public async virtual Task<Response> AsyncDecoratedMethodWithTaskOfTResponseAndCancellationToken(Request request, CancellationToken cancel)
				{ return await LongRunningTask.Instance<Response>(); }

			public static async Task StaticMethod(Request request)
				{ await LongRunningTask.Instance(); }

			public async virtual Task AsyncDecoratedMethodWithCancellationTokenAndOtherParameters(Request request, CancellationToken cancel, object other)
				{ await LongRunningTask.Instance(); }

			public async virtual Task AsyncDecoratedMethodWithCancellationTokenInWrongPlace(CancellationToken cancel, Request request)
				{ await LongRunningTask.Instance(); }

			public virtual Task NonAsyncDecoratedMethodWithCancellationTokenAndOtherParameters(Request request, CancellationToken cancel, object other)
				{ return LongRunningTask.Instance(); }

			public virtual Task NonAsyncDecoratedMethodWithCancellationTokenInWrongPlace(CancellationToken cancel, Request request)
				{ return LongRunningTask.Instance(); }
		}

		public class Request { }

		public class Response { }

		private const int AsyncSafetyTimeoutSeconds = 10;

		private const int AsyncSafetyTimeoutMilliseconds = AsyncSafetyTimeoutSeconds * 1000;

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new AsyncTaskServiceMethodInvocation().Invoking(x => x.CanCreateInvocationDelegateFor(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyInlineAutoData(typeof(StubServiceUnhandled))]
		public void CanCreateInvocationDelegateFor_CalledForAnyMethodInThisType_ExpectFalseIsReturned(Type serviceType)
		{
			serviceType.GetAllDeclaredMethods().ForEach(ExpectCanNotCreateInvocationDelegateFor);
		}

		private static void ExpectCanNotCreateInvocationDelegateFor(MethodInfo method)
		{
			new AsyncTaskServiceMethodInvocation().CanCreateInvocationDelegateFor(method)
				.Should().BeFalse(method + " is not of the correct signature");
		}

		[Theory, NancyInlineAutoData(typeof(StubService))]
		public void CanCreateInvocationDelegateFor_CalledForAnyMethodInThisType_ExpectTrueIsReturned(Type serviceType)
		{
			serviceType.GetAllDeclaredMethods().ForEach(ExpectCanCreateInvocationDelegateFor);
		}

		private static void ExpectCanCreateInvocationDelegateFor(MethodInfo method)
		{
			new AsyncTaskServiceMethodInvocation().CanCreateInvocationDelegateFor(method)
				.Should().BeTrue(method + " is of the correct signature");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName(
			ServiceMethodInvocationContext context)
		{
			new AsyncTaskServiceMethodInvocation().Invoking(x => x.CreateInvocationDelegate(null, context))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Fact]
		public void CreateInvocationDelegate_CalledWithNullContext_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new AsyncTaskServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(
					InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithTaskResponse(null)),
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
			new AsyncTaskServiceMethodInvocation().Invoking(
				x => x.CreateInvocationDelegate(invalidServiceMethod, context))
					.ShouldThrow<ArgumentException>()
					.And.ParamName.Should().Be("serviceMethod", invalidServiceMethod + " is not of the correct signature");
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithSingleParameter_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, object request, Request dto, object defaultResponse)
		{
			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithTaskResponse(null)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Result.Should().BeSameAs(defaultResponse);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithSingleParameter_ExpectServiceMethodCalledAndAwaitedBeforeResponse(
			object request, Request dto, Response defaultResponse)
		{
			var wasCalledWithCorrectArguments = new ManualResetEvent(false);
			var service = CreateStrictMockServiceForDelayedEvent(
				x => x.AsyncDecoratedMethodWithTaskResponse(Arg<Request>.Is.Same(dto)),
				wasCalledWithCorrectArguments);

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithTaskResponse(null)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Wait();
			wasCalledWithCorrectArguments.WaitOne(TimeSpan.FromTicks(1)).Should().BeTrue();
		}

		private static StubService CreateStrictMockServiceForDelayedEvent(Function<StubService, Task> expectation, ManualResetEvent eventFlag)
		{
			var service = MockRepository.GenerateStrictMock<StubService>();
			service.Expect(expectation).Do(new Func<Request, Task>(async x => { await Task.Delay(50); eventFlag.Set(); }));
			return service;
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNonAsyncDecoratedMethodWithSingleParameter_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, object request, Request dto, object defaultResponse)
		{
			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.NonAsyncDecoratedMethodWithTaskResponse(null)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Result.Should().BeSameAs(defaultResponse);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNonAsyncDecoratedMethodWithSingleParameter_ExpectServiceMethodCalledAndAwaitedBeforeResponse(
			object request, Request dto, Response defaultResponse)
		{
			var wasCalledWithCorrectArguments = new ManualResetEvent(false);
			var service = CreateStrictMockServiceForDelayedEvent(
				x => x.NonAsyncDecoratedMethodWithTaskResponse(Arg<Request>.Is.Same(dto)),
				wasCalledWithCorrectArguments);

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.NonAsyncDecoratedMethodWithTaskResponse(null)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Wait();
			wasCalledWithCorrectArguments.WaitOne(TimeSpan.FromTicks(1)).Should().BeTrue();
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithCancellationToken_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, object request, Request dto, object defaultResponse)
		{
			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithCancellationTokenAndTaskResponse(null, CancellationToken.None)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Result.Should().BeSameAs(defaultResponse);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithAsyncDecoratedMethodWithCancellationToken_ExpectServiceMethodCalledWithTokenAndAwaitedBeforeResponse(
			object request, Request dto, Response defaultResponse)
		{
			var wasCalledWithCorrectArguments = new ManualResetEvent(false);
			var service = CreateStrictMockServiceForCancellationEvent(
				x => x.AsyncDecoratedMethodWithCancellationTokenAndTaskResponse(Arg<Request>.Is.Same(dto), Arg<CancellationToken>.Is.Anything),
				wasCalledWithCorrectArguments);

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.AsyncDecoratedMethodWithCancellationTokenAndTaskResponse(null, CancellationToken.None)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
			lambda(request, cancel.Token).Wait();
			wasCalledWithCorrectArguments.WaitOne(TimeSpan.FromTicks(1)).Should().BeTrue();
		}

		private static StubService CreateStrictMockServiceForCancellationEvent(Function<StubService, Task> expectation, ManualResetEvent eventFlag)
		{
			var service = MockRepository.GenerateStrictMock<StubService>();
			service.Expect(expectation).Do(new Func<Request, CancellationToken, Task>((x, c) =>
				{
					c.WaitHandle.WaitOne(TimeSpan.FromSeconds(AsyncSafetyTimeoutSeconds + 1));
					return Task.FromResult(eventFlag.Set());
				}));

			return service;
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNonAsyncDecoratedMethodWithCancellationToken_ExpectReturnedDelegateReturnsDefaultResponse(
			StubService service, object request, Request dto, object defaultResponse)
		{
			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.NonAsyncDecoratedMethodWithCancellationTokenAndTaskResponse(null, CancellationToken.None)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			lambda(request, CancellationToken.None).Result.Should().BeSameAs(defaultResponse);
		}

		[Theory(Timeout = AsyncSafetyTimeoutMilliseconds), NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNonAsyncDecoratedMethodWithCancellationToken_ExpectServiceMethodCalledWithTokenAndAwaitedBeforeResponse(
			object request, Request dto, Response defaultResponse)
		{
			var wasCalledWithCorrectArguments = new ManualResetEvent(false);
			var service = CreateStrictMockServiceForCancellationEvent(
				x => x.NonAsyncDecoratedMethodWithCancellationTokenAndTaskResponse(Arg<Request>.Is.Same(dto), Arg<CancellationToken>.Is.Anything),
				wasCalledWithCorrectArguments);

			var lambda = (Func<object, CancellationToken, Task<object>>) new AsyncTaskServiceMethodInvocation().CreateInvocationDelegate(
				InfoOf.Method<StubService>(svc => svc.NonAsyncDecoratedMethodWithCancellationTokenAndTaskResponse(null, CancellationToken.None)),
				Stub.InvocationContextFor(service, request, dto, defaultResponse));

			var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
			lambda(request, cancel.Token).Wait();
			wasCalledWithCorrectArguments.WaitOne(TimeSpan.FromTicks(1)).Should().BeTrue();
		}
	}

	#pragma warning restore 4014
}
