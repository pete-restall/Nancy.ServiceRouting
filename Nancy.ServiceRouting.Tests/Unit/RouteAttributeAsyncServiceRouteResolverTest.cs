﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	#pragma warning disable 4014

	public class RouteAttributeAsyncServiceRouteResolverTest
	{
		private static Task LongRunningTask() { return null; }

		private class ServiceContainingOnlyNonPublicMethods
		{
			private async Task<Response> ServiceMethod(RequestOne request) { await LongRunningTask(); return new Response(); }
			protected async Task<Response> ServiceMethod(RequestTwo request) { await LongRunningTask(); return new Response(); }
			internal async Task<Response> ServiceMethod(RequestThree request) { await LongRunningTask(); return new Response(); }
			protected internal async Task<Response> ServiceMethod(RequestFour request) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceContainingMixtureOfPublicAndNonPublicMethods
		{
			private async Task<Response> ServiceMethod(RequestOne request) { await LongRunningTask(); return new Response(); }
			public async Task<Response> ServiceMethod(RequestTwo request) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceContainingMixtureOfServiceAndNonServiceMethods
		{
			public async Task<Response> ServiceMethod(RequestOne request, int otherParameter) { await LongRunningTask(); return new Response(); }
			public async Task<Response> ServiceMethod(RequestTwo request) { await LongRunningTask(); return new Response(); }
			public async Task<Response> ServiceMethod(RequestTwo request, CancellationToken cancel, int tooMany) { await LongRunningTask(); return new Response(); }
			public async Task<Response> ServiceMethod(int otherParameter, RequestThree request) { await LongRunningTask(); return new Response(); }
			public async Task<Response> ServiceMethod(RequestFour request) { await LongRunningTask(); return new Response(); }
			public async Task<Response> ServiceMethod() { await LongRunningTask(); return new Response(); }
			public async Task<Response> ServiceMethod(NonDecoratedRequest request) { await LongRunningTask(); return new Response(); }
			public Task<Response> ServiceMethod(RequestFive request) { return null; }
		}

		private class ServiceContainingMultipleRouteVerbs
		{
			public async Task<Response> ServiceMethod(RequestFive request) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceContainingOnlyStaticMethods
		{
			public static async Task<Response> ServiceMethod(RequestTwo request) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceInheritingFromAnotherType: ServiceContainingMixtureOfPublicAndNonPublicMethods
		{
			public async Task<Response> MethodFromDerivedService(RequestThree request) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceContainingServiceMethodWithNoReturnValue
		{
			public async void ServiceMethod(RequestOne request) { await LongRunningTask(); }
		}

		private class ServiceContainingConstructorMatchingServiceMethodSignature
		{
			public ServiceContainingConstructorMatchingServiceMethodSignature(RequestOne request) { }
		}

		private abstract class AbstractService
		{
			public abstract Task<Response> ServiceMethod(RequestOne request);
			public async Task<Response> ServiceMethod(RequestTwo request) { await LongRunningTask(); return new Response(); }
		}

		private interface IService
		{
			Task<Response> ServiceMethod(RequestOne request);
		}

		private class GenericService<T>
		{
			public async void ServiceMethod(T request) { await LongRunningTask(); }
		}

		private class ServiceContainingGenericServiceMethod
		{
			public async void ServiceMethod<T>(T request) { await LongRunningTask(); }
		}

		private class ServiceContainingImplementationsOfAbstractServiceMethods: AbstractService
		{
			public async override Task<Response> ServiceMethod(RequestOne request) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceContainingImplementationsOfInterfaceServiceMethods: IService
		{
			public async Task<Response> ServiceMethod(RequestOne request) { await LongRunningTask(); return new Response(); }
			public async Task<Response> ServiceMethod(RequestTwo request) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceContainingShadowServiceMethods: GenericService<RequestOne>
		{
			public async new Task<Response> ServiceMethod(RequestOne request) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceUsingDtoDecoratedWithMultipleRoutes
		{
			public async void ServiceMethod(RequestMultipleRoutes request) { await LongRunningTask(); }
		}

		private class ServiceContainingServiceMethodsWithCancellationTokens
		{
			public async void ServiceMethodReturningVoid(RequestOne request, CancellationToken cancel) { await LongRunningTask(); }
			public async Task ServiceMethodReturningTask(RequestTwo request, CancellationToken cancel) { await LongRunningTask(); }
			public async Task<object> ServiceMethodReturningTaskOfT(RequestThree request, CancellationToken cancel) { await LongRunningTask(); return new Response(); }
		}

		private class ServiceContainingNonAsyncServiceMethodsThatReturnTasks
		{
			public void ServiceMethod(RequestOne request) { }
			public Task ServiceMethod(RequestTwo request) { return LongRunningTask(); }
			public Task<object> ServiceMethod(RequestThree request) { return null; }
			public void ServiceMethod(RequestOne request, CancellationToken cancel) { }
			public Task ServiceMethod(RequestTwo request, CancellationToken cancel) { return LongRunningTask(); }
			public Task<object> ServiceMethod(RequestThree request, CancellationToken cancel) { return null; }
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
			new RouteAttributeAsyncServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(AbstractService)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithInterfaceServiceType_ExpectArgumentExceptionWithCorrectParamName()
		{
			new RouteAttributeAsyncServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(IService)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithGenericDefinitionServiceType_ExpectArgumentExceptionWithCorrectParamName()
		{
			new RouteAttributeAsyncServiceRouteResolver().Invoking(x => x.GetServiceRoutes(typeof(GenericService<>)))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithNullServiceType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new RouteAttributeAsyncServiceRouteResolver().Invoking(x => x.GetServiceRoutes(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingOnlyNonPublicServiceMethods_ExpectEmptyEnumerableIsReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingOnlyNonPublicMethods>().Should().BeEmpty();
		}

		private static IEnumerable<Route> ResolvedServiceRoutesFor<T>()
		{
			return new RouteAttributeAsyncServiceRouteResolver().GetServiceRoutes(typeof(T));
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
		public void GetServiceRoutes_CalledWithServiceTypeContainingServiceMethodsWithCancellationTokens_ExpectServiceMethodsAreReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingServiceMethodsWithCancellationTokens>()
				.ShouldBeEquivalentTo(
					new Route(
						"GET",
						"/requestone",
						InfoOf.Method<ServiceContainingServiceMethodsWithCancellationTokens>(
							x => x.ServiceMethodReturningVoid(null, CancellationToken.None))),
					new Route(
						"GET",
						"/requesttwo",
						InfoOf.Method<ServiceContainingServiceMethodsWithCancellationTokens>(
							x => x.ServiceMethodReturningTask(null, CancellationToken.None))),
					new Route(
						"GET",
						"/requestthree",
						InfoOf.Method<ServiceContainingServiceMethodsWithCancellationTokens>(
							x => x.ServiceMethodReturningTaskOfT(null, CancellationToken.None))));
		}

		[Fact]
		public void GetServiceRoutes_CalledWithServiceTypeContainingNonAsyncServiceMethodsThatReturnTasks_EmptyNonAsyncMethodsAreNotReturned()
		{
			ResolvedServiceRoutesFor<ServiceContainingNonAsyncServiceMethodsThatReturnTasks>().Should().BeEmpty();
		}
	}

	#pragma warning restore 4014
}
