using System;
using FluentAssertions;
using Nancy;
using Nancy.Testing;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public class RouteRegistrarDispatchContextTest
	{
		public class StubModule: NancyModule
		{
			public StubModule() { }
		}

		public class StubService
		{
			public virtual object StubServiceMethod(Request request) { return new object(); }
			public virtual object StubNonServiceMethod(object request) { return new object(); }
			public virtual void StubServiceMethodWithNoResponse(VoidRequest request) { }
		}

		public class AnotherStubService
		{
			public virtual object StubServiceMethod(Request request) { return new object(); }
		}

		[Route("/whatever")]
		public class Request { }

		[Route("/void")]
		public class VoidRequest { }

		[Theory, NancyAutoData]
		public void ExpectRegistrarDispatchContextCanBeOverridden(RouteRegistrar registrar, Guid token)
		{
			var module = new StubModule();
			var service = CreateStubServiceToReturn(token);
			var factory = CreateStubServiceFactoryFor(service);

			registrar
				.WithDispatchContext(x => x.WithServiceFactory(factory))
				.RegisterServiceInto(module, typeof(StubService));

			new Browser(with => with.Module(module)).Get<Guid>("/whatever").Should().Be(token);
		}

		private static StubService CreateStubServiceToReturn(Guid token)
		{
			var service = MockRepository.GenerateStub<StubService>();
			service.Stub(x => x.StubServiceMethod(Arg<Request>.Is.Anything)).Return(token);
			return service;
		}

		private static Func<Type, object> CreateStubServiceFactoryFor(StubService service)
		{
			var factory = MockRepository.GenerateStub<Func<Type, object>>();
			factory.Stub(x => x(Arg<Type>.Is.Equal(typeof(StubService)))).Return(service);
			return factory;
		}

		[Theory, NancyAutoData]
		public void ExpectMethodCannotBeOverriddenByModifyingDispatchContext(RouteRegistrar registrar, Guid goodToken, Guid badToken)
		{
			var module = new StubModule();
			var service = CreateStubServiceToReturn(goodToken);
			service.Stub(x => x.StubNonServiceMethod(Arg<Request>.Is.Anything)).Return(badToken);
			var factory = CreateStubServiceFactoryFor(service);

			registrar
				.WithDispatchContext(x => x
					.WithServiceFactory(factory)
					.WithMethod(InfoOf.Method<StubService>(svc => svc.StubNonServiceMethod(null))))
				.RegisterServiceInto(module, typeof(StubService));

			new Browser(with => with.Module(module)).Get<Guid>("/whatever").Should().Be(goodToken);
		}

		[Theory, NancyAutoData]
		public void ExpectServiceTypeCannotBeOverriddenByModifyingDispatchContext(RouteRegistrar registrar, Guid token)
		{
			var module = new StubModule();
			var service = CreateStubServiceToReturn(token);
			var factory = CreateStubServiceFactoryFor(service);

			registrar
				.WithDispatchContext(x => x
					.WithServiceFactory(factory)
					.WithServiceType(typeof(AnotherStubService)))
				.RegisterServiceInto(module, typeof(StubService));

			new Browser(with => with.Module(module)).Get<Guid>("/whatever").Should().Be(token);
		}

		[Theory, NancyAutoData]
		public void ExpectModuleCannotBeOverriddenByModifyingDispatchContext(RouteRegistrar registrar, Guid token)
		{
			var module = new StubModule();
			var service = CreateStubServiceToReturn(token);
			var factory = CreateStubServiceFactoryFor(service);

			registrar
				.WithDispatchContext(x => x
					.WithServiceFactory(factory)
					.WithModule(new StubModule()))
				.RegisterServiceInto(module, typeof(StubService));

			new Browser(with => with.Module(module)).Get<Guid>("/whatever").Should().Be(token);
		}

		[Theory, NancyAutoData]
		public void ExpectOriginalDispatchContextRemainsUnmodifiedWhenOverridden(RouteRegistrar registrar, Guid token)
		{
			var module = new StubModule();
			var factory = MockRepository.GenerateMock<Func<Type, object>>();

			registrar
				.WithDispatchContext(x => x.WithServiceFactory(factory))
				.RegisterServiceInto(module, typeof(StubService));

			var anotherModule = new StubModule();
			registrar.RegisterServiceInto(anotherModule, typeof(StubService));

			new Browser(with => with.Module(anotherModule)).Get<object>("/whatever");
			factory.AssertWasNotCalled(x => x(Arg<Type>.Is.Anything));
		}

		[Theory, NancyAutoData]
		public void ExpectOverridingRegistrarDispatchContextMultipleTimesInheritsSettingsFromPreviousContext(RouteRegistrar registrar, Guid token)
		{
			var module = new StubModule();
			var service = new StubService();
			var factory = MockRepository.GenerateMock<Func<Type, object>>();
			factory.Stub(x => x(Arg<Type>.Is.Anything)).Return(service);

			registrar
				.WithDispatchContext(x => x.WithServiceFactory(factory))
					.WithDispatchContext(x => x.WithDefaultResponse(token))
					.RegisterServiceInto(module, typeof(StubService));

			new Browser(with => with.Module(module)).Get<Guid>("/void").Should().Be(token);
			factory.AssertWasCalled(x => x(Arg<Type>.Is.Equal(typeof(StubService))), x => x.Repeat.Once());
		}
	}
}
