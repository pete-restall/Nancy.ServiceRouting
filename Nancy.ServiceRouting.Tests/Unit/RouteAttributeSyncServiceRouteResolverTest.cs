using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class RouteAttributeSyncServiceRouteResolverTest
	{
		private class ServiceContainingOnlyNonPublicMethods
		{
			private Response ServiceMethod(RequestOne request) { return new Response(); }
			protected Response ServiceMethod(RequestTwo request) { return new Response(); }
			internal Response ServiceMethod(RequestThree request) { return new Response(); }
			protected internal Response ServiceMethod(RequestFour request) { return new Response(); }
		}

		private class ServiceContainingMixtureOfPublicAndNonPublicMethods
		{
			private Response ServiceMethod(RequestOne request) { return new Response(); }
			public Response ServiceMethod(RequestTwo request) { return new Response(); }
		}

		private class ServiceContainingMixtureOfServiceAndNonServiceMethods
		{
			public Response ServiceMethod(RequestOne request, int otherParameter) { return new Response(); }
			public Response ServiceMethod(RequestTwo request) { return new Response(); }
			public Response ServiceMethod(int otherParameter, RequestThree request) { return new Response(); }
			public Response ServiceMethod(RequestFour request) { return new Response(); }
			public Response ServiceMethod() { return new Response(); }
			public Response ServiceMethod(NonDecoratedRequest request) { return new Response(); }
		}

		private class ServiceContainingMultipleRouteVerbs
		{
			public Response ServiceMethod(RequestFive request) { return new Response(); }
		}

		private class ServiceContainingOnlyStaticMethods
		{
			public static Response ServiceMethod(RequestTwo request) { return new Response(); }
		}

		private class ServiceInheritingFromAnotherType: ServiceContainingMixtureOfPublicAndNonPublicMethods
		{
			public Response MethodFromDerivedService(RequestThree request) { return new Response(); }
		}

		private class ServiceContainingServiceMethodWithNoReturnValue
		{
			public void ServiceMethod(RequestOne request) { }
		}

		private class ServiceContainingConstructorMatchingServiceMethodSignature
		{
			public ServiceContainingConstructorMatchingServiceMethodSignature(RequestOne request) { }
		}

		private abstract class AbstractService
		{
			public abstract Response ServiceMethod(RequestOne request);
			public Response ServiceMethod(RequestTwo request) { return new Response(); }
		}

		private interface IService
		{
			Response ServiceMethod(RequestOne request);
		}

		private class GenericService<T>
		{
			public void ServiceMethod(T request) { }
		}

		private class ServiceContainingGenericServiceMethod
		{
			public void ServiceMethod<T>(T request) { }
		}

		private class ServiceContainingImplementationsOfAbstractServiceMethods: AbstractService
		{
			public override Response ServiceMethod(RequestOne request) { return new Response(); }
		}

		private class ServiceContainingImplementationsOfInterfaceServiceMethods: IService
		{
			public Response ServiceMethod(RequestOne request) { return new Response(); }
			public Response ServiceMethod(RequestTwo request) { return new Response(); }
		}

		private class ServiceContainingShadowServiceMethods: GenericService<RequestOne>
		{
			public new Response ServiceMethod(RequestOne request) { return new Response(); }
		}

		private class ServiceUsingDtoDecoratedWithMultipleRoutes
		{
			public void ServiceMethod(RequestMultipleRoutes request) { }
		}

		private class ServiceContainingAsyncServiceMethods
		{
			public async void ServiceMethodReturningVoid(RequestOne request) { await LongRunningTask(); }
			private static Task LongRunningTask() { return null; }
			public async Task ServiceMethodReturningTask(RequestTwo request) { await LongRunningTask(); }
			public async Task<object> ServiceMethodReturningTaskOfT(RequestThree request) { await LongRunningTask(); return new Response(); }
		}

		[Route("/requestone")]
		private class RequestOne { }

		[Route("/requesttwo")]
		private class RequestTwo { }

		[Route("/requestthree")]
		private class RequestThree { }

		[Route("/requestfour")]
		private class RequestFour { }

		[Route("/requestfive", "ABC", "DEF", "GHI")]
		private class RequestFive { }

		[Route("/request/multiple/1")]
		[Route("/request/multiple/2")]
		private class RequestMultipleRoutes { }

		private class NonDecoratedRequest { }

		private class Response { }

		[Fact]
		public void GetServiceRoutes_CalledWithAbstractServiceType_ExpectArgumentExceptionWithCorrectParamName()
		{
			new RouteAttributeSyncServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(AbstractService)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithInterfaceServiceType_ExpectArgumentExceptionWithCorrectParamName()
		{
			new RouteAttributeSyncServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(IService)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithGenericDefinitionServiceType_ExpectArgumentExceptionWithCorrectParamName()
		{
			new RouteAttributeSyncServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(GenericService<>)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithNullServiceType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new RouteAttributeSyncServiceRouteResolver().Invoking(x => x.GetServiceRoutes(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingOnlyNonPublicServiceMethods_ExpectEmptyEnumerableIsReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingOnlyNonPublicMethods>().Should().BeEmpty();
		}

		private static IEnumerable<Route> ResolvedServiceRoutesFor<T>()
		{
			return new RouteAttributeSyncServiceRouteResolver().GetServiceRoutes(typeof(T));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingMixtureOfPublicAndNonPublicServiceMethods_ExpectOnlyPublicMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingMixtureOfPublicAndNonPublicMethods>()
				.ShouldBeEquivalentTo(
					new Route(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingMixtureOfPublicAndNonPublicMethods>(x => x.ServiceMethod(null))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingMixtureOfServiceAndNonServiceMethods_ExpectOnlyServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingMixtureOfServiceAndNonServiceMethods>()
				.ShouldBeEquivalentTo(
					new Route(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingMixtureOfServiceAndNonServiceMethods>(x => x.ServiceMethod(new RequestTwo()))),
					new Route(
						"GET",
						"/requestfour",
						InfoOf.Method<ServiceContainingMixtureOfServiceAndNonServiceMethods>(x => x.ServiceMethod(new RequestFour()))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingMultipleRouteVerbs_ExpectServiceRoutesForEachVerbAreReturned()
		{
			const string routePath = "/requestfive";
			var serviceMethod = InfoOf.Method<ServiceContainingMultipleRouteVerbs>(x => x.ServiceMethod(new RequestFive()));
			ResolvedServiceRoutesFor<ServiceContainingMultipleRouteVerbs>()
				.ShouldBeEquivalentTo(
					new Route("ABC", routePath, serviceMethod),
					new Route("DEF", routePath, serviceMethod),
					new Route("GHI", routePath, serviceMethod));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingStaticMethods_ExpectStaticMethodsAreNotReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingOnlyStaticMethods>().Should().BeEmpty();
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeInheritingFromAnother_ExpectServiceMethodsFromBaseAndDerivedTypeAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceInheritingFromAnotherType>()
				.ShouldBeEquivalentTo(
					new Route("GET", "/requesttwo", InfoOf.Method<ServiceInheritingFromAnotherType>(x => x.ServiceMethod(new RequestTwo()))),
					new Route("GET", "/requestthree", InfoOf.Method<ServiceInheritingFromAnotherType>(x => x.MethodFromDerivedService(new RequestThree()))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingServiceMethodWithNoReturnValue_ExpectServiceMethodIsStillReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingServiceMethodWithNoReturnValue>()
				.ShouldBeEquivalentTo(
					new Route(
						"GET",
						"/requestone",
						InfoOf.Method<ServiceContainingServiceMethodWithNoReturnValue>(x => x.ServiceMethod(null))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingConstructorMatchingServiceMethodSignature_ExpectConstructorIsNotReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingConstructorMatchingServiceMethodSignature>().Should().BeEmpty();
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingImplementationsOfAbstractServiceMethods_ExpectAbstractAndDeclaredServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingImplementationsOfAbstractServiceMethods>()
				.ShouldBeEquivalentTo(
					new Route(
						"GET",
						"/requestone",
						InfoOf.Method<ServiceContainingImplementationsOfAbstractServiceMethods>(x => x.ServiceMethod(new RequestOne()))),
					new Route(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingImplementationsOfAbstractServiceMethods>(x => x.ServiceMethod(new RequestTwo()))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingImplementationsOfInterfaceServiceMethods_ExpectInterfaceAndDeclaredServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingImplementationsOfInterfaceServiceMethods>()
				.ShouldBeEquivalentTo(
					new Route(
						"GET",
						"/requestone",
						InfoOf.Method<ServiceContainingImplementationsOfInterfaceServiceMethods>(x => x.ServiceMethod(new RequestOne()))),
					new Route(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingImplementationsOfInterfaceServiceMethods>(x => x.ServiceMethod(new RequestTwo()))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithGenericServiceType_ExpectConcreteServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<GenericService<RequestOne>>()
				.ShouldBeEquivalentTo(new Route("GET", "/requestone", InfoOf.Method<GenericService<RequestOne>>(x => x.ServiceMethod(null))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingGenericServiceMethod_ExpectGenericServiceMethodsAreNotReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingGenericServiceMethod>().Should().BeEmpty();
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingShadowedServiceMethods_ExpectNonShadowedServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingShadowServiceMethods>()
				.ShouldBeEquivalentTo(new Route("GET", "/requestone", InfoOf.Method<ServiceContainingShadowServiceMethods>(x => x.ServiceMethod(null))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeUsingDtoDecoratedWithMultipleRoutes_ExpectAllRoutesFromDtoAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceUsingDtoDecoratedWithMultipleRoutes>()
				.ShouldBeEquivalentTo(
					new Route("GET", "/request/multiple/1", InfoOf.Method<ServiceUsingDtoDecoratedWithMultipleRoutes>(x => x.ServiceMethod(null))),
					new Route("GET", "/request/multiple/2", InfoOf.Method<ServiceUsingDtoDecoratedWithMultipleRoutes>(x => x.ServiceMethod(null))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingAsyncServiceMethods_ExpectAsyncServiceMethodsAreNotReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingAsyncServiceMethods>().Should().BeEmpty();
		}
	}
}
