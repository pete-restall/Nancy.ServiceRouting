using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Nancy.ServiceRouting.Tests.Unit
{
	public class RouteAttributeServiceRouteResolverTest
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

		private class ServiceContainingShadowServiceMethods : GenericService<RequestOne>
		{
			public new Response ServiceMethod(RequestOne request) { return new Response(); }
		}

		private class ServiceUsingDtoDecoratedWithMultipleRoutes
		{
			public void ServiceMethod(RequestMultipleRoutes request) { }
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
			new RouteAttributeServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(AbstractService)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithInterfaceServiceType_ExpectArgumentExceptionWithCorrectParamName()
		{
			new RouteAttributeServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(IService)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithGenericDefinitionServiceType_ExpectArgumentExceptionWithCorrectParamName()
		{
			new RouteAttributeServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(GenericService<>)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithNullServiceType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new RouteAttributeServiceRouteResolver().Invoking(x => x.GetServiceRoutes(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingOnlyNonPublicServiceMethods_ExpectEmptyEnumerableIsReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingOnlyNonPublicMethods>().Should().BeEmpty();
		}

		private static IEnumerable<ServiceRoute> ResolvedServiceRoutesFor<T>()
		{
			return new RouteAttributeServiceRouteResolver().GetServiceRoutes(typeof(T));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingMixtureOfPublicAndNonPublicServiceMethods_ExpectOnlyPublicMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingMixtureOfPublicAndNonPublicMethods>()
				.ShouldBeEquivalentTo(
					new ServiceRoute(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingMixtureOfPublicAndNonPublicMethods>(x => x.ServiceMethod(null))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingMixtureOfServiceAndNonServiceMethods_ExpectOnlyServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingMixtureOfServiceAndNonServiceMethods>()
				.ShouldBeEquivalentTo(
					new ServiceRoute(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingMixtureOfServiceAndNonServiceMethods>(x => x.ServiceMethod(new RequestTwo()))),
					new ServiceRoute(
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
					new ServiceRoute("ABC", routePath, serviceMethod),
					new ServiceRoute("DEF", routePath, serviceMethod),
					new ServiceRoute("GHI", routePath, serviceMethod));
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
					new ServiceRoute("GET", "/requesttwo", InfoOf.Method<ServiceInheritingFromAnotherType>(x => x.ServiceMethod(new RequestTwo()))),
					new ServiceRoute("GET", "/requestthree", InfoOf.Method<ServiceInheritingFromAnotherType>(x => x.MethodFromDerivedService(new RequestThree()))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingServiceMethodWithNoReturnValue_ExpectServiceMethodIsStillReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingServiceMethodWithNoReturnValue>()
				.ShouldBeEquivalentTo(
					new ServiceRoute(
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
					new ServiceRoute(
						"GET",
						"/requestone",
						InfoOf.Method<ServiceContainingImplementationsOfAbstractServiceMethods>(x => x.ServiceMethod(new RequestOne()))),
					new ServiceRoute(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingImplementationsOfAbstractServiceMethods>(x => x.ServiceMethod(new RequestTwo()))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingImplementationsOfInterfaceServiceMethods_ExpectInterfaceAndDeclaredServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingImplementationsOfInterfaceServiceMethods>()
				.ShouldBeEquivalentTo(
					new ServiceRoute(
						"GET",
						"/requestone",
						InfoOf.Method<ServiceContainingImplementationsOfInterfaceServiceMethods>(x => x.ServiceMethod(new RequestOne()))),
					new ServiceRoute(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingImplementationsOfInterfaceServiceMethods>(x => x.ServiceMethod(new RequestTwo()))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithGenericServiceType_ExpectConcreteServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<GenericService<RequestOne>>()
				.ShouldBeEquivalentTo(new ServiceRoute("GET", "/requestone", InfoOf.Method<GenericService<RequestOne>>(x => x.ServiceMethod(null))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingGenericServiceMethod_ExpectGenericServiceMethodsIsNotReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingGenericServiceMethod>().Should().BeEmpty();
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingShadowedServiceMethods_ExpectNonShadowedServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingShadowServiceMethods>()
				.ShouldBeEquivalentTo(new ServiceRoute("GET", "/requestone", InfoOf.Method<ServiceContainingShadowServiceMethods>(x => x.ServiceMethod(null))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeUsingDtoDecoratedWithMultipleRoutes_ExpectAllRoutesFromDtoAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceUsingDtoDecoratedWithMultipleRoutes>()
				.ShouldBeEquivalentTo(
					new ServiceRoute("GET", "/request/multiple/1", InfoOf.Method<ServiceUsingDtoDecoratedWithMultipleRoutes>(x => x.ServiceMethod(null))),
					new ServiceRoute("GET", "/request/multiple/2", InfoOf.Method<ServiceUsingDtoDecoratedWithMultipleRoutes>(x => x.ServiceMethod(null))));
		}
	}
}
