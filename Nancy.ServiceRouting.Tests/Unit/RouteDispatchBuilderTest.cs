using System;
using FluentAssertions;
using Nancy;
using Restall.Nancy.ServiceRouting.Sync;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class RouteDispatchBuilderTest
	{
		public class StubService
		{
			public virtual Response ServiceMethod(Request request) { return new Response(); }
			public virtual void ServiceMethodNotReturningResponse(Request request) { }
		}

		public class Request { }

		public class Response { }

		[Fact]
		public void WithServiceFactory_CalledWithNullServiceFactory_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var builder = new RouteDispatchBuilder();
			builder.Invoking(x => x.WithServiceFactory(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceFactory");
		}

		[Fact]
		public void WithServiceFactory_Called_ExpectSameBuilderInstanceIsReturned()
		{
			var builder = new RouteDispatchBuilder();
			builder.WithServiceFactory(x => null).Should().BeSameAs(builder);
		}

		[Fact]
		public void WithServiceMethodInvocation_CalledWithNullServiceMethodInvocation_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var builder = new RouteDispatchBuilder();
			builder.Invoking(x => x.WithServiceMethodInvocation(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethodInvocation");
		}

		[Fact]
		public void WithServiceMethodInvocation_Called_ExpectSameBuilderInstanceIsReturned()
		{
			var builder = new RouteDispatchBuilder();
			builder.WithServiceMethodInvocation(DummyServiceMethodInvocation).Should().BeSameAs(builder);
		}

		private static IServiceMethodInvocation DummyServiceMethodInvocation
		{
			get { return MockRepository.GenerateStub<IServiceMethodInvocation>(); }
		}

		[Fact]
		public void WithRequestMessageBinder_CalledWithNullRequestMessageBinder_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var builder = new RouteDispatchBuilder();
			builder.Invoking(x => x.WithRequestMessageBinder(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestMessageBinder");
		}

		[Fact]
		public void WithRequestMessageBinder_Called_ExpectSameBuilderInstanceIsReturned()
		{
			var builder = new RouteDispatchBuilder();
			builder.WithRequestMessageBinder(DummyRequestMessageBinder).Should().BeSameAs(builder);
		}

		private static IServiceRequestBinder DummyRequestMessageBinder
		{
			get { return MockRepository.GenerateStub<IServiceRequestBinder>(); }
		}

		[Fact]
		public void WithModule_CalledWithNullModule_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var builder = new RouteDispatchBuilder();
			builder.Invoking(x => x.WithModule(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("module");
		}

		[Theory, NancyAutoData]
		public void WithModule_Called_ExpectSameBuilderInstanceIsReturned(NancyModule module)
		{
			var builder = new RouteDispatchBuilder();
			builder.WithModule(module).Should().BeSameAs(builder);
		}

		[Fact]
		public void WithServiceType_CalledWithNullServiceType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var builder = new RouteDispatchBuilder();
			builder.Invoking(x => x.WithServiceType(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void WithServiceType_Called_ExpectSameBuilderInstanceIsReturned()
		{
			var builder = new RouteDispatchBuilder();
			builder.WithServiceType(typeof(StubService)).Should().BeSameAs(builder);
		}

		[Fact]
		public void WithMethod_CalledWithNullMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var builder = new RouteDispatchBuilder();
			builder.Invoking(x => x.WithMethod(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("method");
		}

		[Fact]
		public void WithMethod_Called_ExpectSameBuilderInstanceIsReturned()
		{
			var builder = new RouteDispatchBuilder();
			builder.WithMethod(InfoOf.Method<StubService>(x => x.ServiceMethod(null))).Should().BeSameAs(builder);
		}

		[Fact]
		public void WithDefaultResponse_CalledWithNullResponse_ExpectNoArgumentNullException()
		{
			var builder = new RouteDispatchBuilder();
			builder.Invoking(x => x.WithDefaultResponse(null)).ShouldNotThrow<ArgumentNullException>();
		}

		[Theory, NancyAutoData]
		public void WithDefaultResponse_Called_ExpectSameBuilderInstanceIsReturned(object defaultResponse)
		{
			var builder = new RouteDispatchBuilder();
			builder.WithDefaultResponse(defaultResponse).Should().BeSameAs(builder);
		}

		[Theory, NancyAutoData]
		public void Build_Called_ExpectReturnedDelegateCallsServiceMethodOnFactoryCreatedServiceWithBoundModel(
			NancyModule module, object request, Request message, Response response)
		{
			var requestMessageBinderDelegate = MockRepository.GenerateStub<Func<object, object>>();
			requestMessageBinderDelegate.Stub(x => x(Arg<object>.Is.Same(request))).Return(message);

			var requestMessageBinder = MockRepository.GenerateStub<IServiceRequestBinder>();
			requestMessageBinder.Stub(x => x.CreateBindingDelegate(
				Arg<Type>.Is.Equal(typeof(Request)),
				Arg<ServiceRequestBinderContext>.Matches(ctx => ReferenceEquals(ctx.NancyModule, module))))
					.Return(requestMessageBinderDelegate);

			var service = MockRepository.GenerateStub<StubService>();
			service.Stub(x => x.ServiceMethod(Arg<Request>.Is.Same(message))).Return(response);

			var serviceFactory = MockRepository.GenerateStub<Func<Type, object>>();
			serviceFactory.Stub(x => x(Arg<Type>.Is.Equal(service.GetType()))).Return(service);

			var lambda = (Func<object, object>) new RouteDispatchBuilder()
				.WithServiceFactory(serviceFactory)
				.WithServiceMethodInvocation(new SyncServiceMethodInvocation())
				.WithRequestMessageBinder(requestMessageBinder)
				.WithModule(module)
				.WithServiceType(service.GetType())
				.WithMethod(InfoOf.Method<StubService>(x => x.ServiceMethod(null)))
				.Build();

			lambda(request).Should().BeSameAs(response);
		}

		[Theory, NancyAutoData]
		public void Build_Called_ExpectReturnedDelegateUsesNewServiceInstanceForEachCall(NancyModule module, Request request, Response response)
		{
			var requestMessageBinder = StubRequestMessageBinderToReturn(request);
			var services = new[] {MockRepository.GenerateMock<StubService>(), MockRepository.GenerateMock<StubService>()};

			var serviceFactory = MockRepository.GenerateStub<Func<Type, object>>();
			services.ForEach(service => serviceFactory.Stub(x => x(Arg<Type>.Is.Anything)).Return(service).Repeat.Once());

			var lambda = (Func<object, object>) new RouteDispatchBuilder()
				.WithServiceFactory(serviceFactory)
				.WithServiceMethodInvocation(new SyncServiceMethodInvocation())
				.WithRequestMessageBinder(requestMessageBinder)
				.WithModule(module)
				.WithServiceType(services[0].GetType())
				.WithMethod(InfoOf.Method<StubService>(x => x.ServiceMethod(null)))
				.Build();

			services.ForEach(service => lambda(request));
			services.ForEach(service => service.AssertWasCalled(x => x.ServiceMethod(Arg<Request>.Is.Anything), x => x.Repeat.Once()));
		}

		private static IServiceRequestBinder StubRequestMessageBinderToReturn(Request request)
		{
			var requestMessageBinder = MockRepository.GenerateStub<IServiceRequestBinder>();
			requestMessageBinder.Stub(x => x.CreateBindingDelegate(
				Arg<Type>.Is.Anything, Arg<ServiceRequestBinderContext>.Is.Anything)).Return(x => request);

			return requestMessageBinder;
		}

		[Theory, NancyAutoData]
		public void Build_CalledWhenDefaultResponseSetForServiceMethodNotReturningResponse_ExpectReturnedDelegateReturnsDefaultResponse(
			NancyModule module, object requestParameters, Request request, Response defaultResponse)
		{
			var lambda = (Func<object, object>) new RouteDispatchBuilder()
				.WithServiceFactory(x => new StubService())
				.WithServiceMethodInvocation(new SyncServiceMethodInvocation())
				.WithRequestMessageBinder(StubRequestMessageBinderToReturn(request))
				.WithModule(module)
				.WithServiceType(typeof(StubService))
				.WithMethod(InfoOf.Method<StubService>(x => x.ServiceMethodNotReturningResponse(null)))
				.WithDefaultResponse(defaultResponse)
				.Build();

			lambda(requestParameters).Should().BeSameAs(defaultResponse);
		}

		[Theory, NancyAutoData]
		public void Build_CalledWhenDefaultResponseNotSetForServiceMethodNotReturningResponse_ExpectReturnedDelegateReturnsHttpNoContentResponse(
			NancyModule module, object requestParameters, Request request)
		{
			var lambda = (Func<object, object>) new RouteDispatchBuilder()
				.WithServiceFactory(x => new StubService())
				.WithServiceMethodInvocation(new SyncServiceMethodInvocation())
				.WithRequestMessageBinder(StubRequestMessageBinderToReturn(request))
				.WithModule(module)
				.WithServiceType(typeof(StubService))
				.WithMethod(InfoOf.Method<StubService>(x => x.ServiceMethodNotReturningResponse(null)))
				.Build();

			lambda(requestParameters).Should().Be(HttpStatusCode.NoContent);
		}
	}
}
