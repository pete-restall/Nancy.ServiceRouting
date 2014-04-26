using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nancy;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class RouteRegistrarTest
	{
		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullRouteTableBuilder_ExpectArgumentNullExceptionWithCorrectParamName(
			RouteDispatchBuilder routeDispatchBuilder)
		{
			Action constructor = () => new RouteRegistrar(
				null, routeDispatchBuilder, DummyServiceFactory, DummyRequestMessageBinder, DummyServiceMethodInvocation);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("routeTableBuilder");
		}

		private static Func<Type, object> DummyServiceFactory
		{
			get { return x => new object(); }
		}

		private static IServiceRequestBinder DummyRequestMessageBinder
		{
			get { return MockRepository.GenerateStub<IServiceRequestBinder>(); }
		}

		private static IServiceMethodInvocation DummyServiceMethodInvocation
		{
			get { return MockRepository.GenerateStub<IServiceMethodInvocation>(); }
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullRouteDispatchBuilder_ExpectArgumentNullExceptionWithCorrectParamName(
			RouteTableBuilder routeTableBuilder)
		{
			Action constructor = () => new RouteRegistrar(
				routeTableBuilder, null, DummyServiceFactory, DummyRequestMessageBinder, DummyServiceMethodInvocation);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("routeDispatchBuilder");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullServiceFactory_ExpectArgumentNullExceptionWithCorrectParamName(
			RouteTableBuilder routeTableBuilder, RouteDispatchBuilder routeDispatchBuilder)
		{
			Action constructor = () => new RouteRegistrar(
				routeTableBuilder, routeDispatchBuilder, null, DummyRequestMessageBinder, DummyServiceMethodInvocation);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceFactory");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullRequestMessageBinder_ExpectArgumentNullExceptionWithCorrectParamName(
			RouteTableBuilder routeTableBuilder, RouteDispatchBuilder routeDispatchBuilder)
		{
			Action constructor = () => new RouteRegistrar(
				routeTableBuilder, routeDispatchBuilder, DummyServiceFactory, null, DummyServiceMethodInvocation);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestMessageBinder");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullServiceMethodInvocation_ExpectArgumentNullExceptionWithCorrectParamName(
			RouteTableBuilder routeTableBuilder, RouteDispatchBuilder routeDispatchBuilder)
		{
			Action constructor = () => new RouteRegistrar(
				routeTableBuilder, routeDispatchBuilder, DummyServiceFactory, DummyRequestMessageBinder, null);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethodInvocation");
		}

		[Theory, NancyAutoData]
		public void RegisterServiceInto_CalledWithNullModule_ExpectArgumentNullExceptionWithCorrectParamName(RouteRegistrar registrar)
		{
			registrar.Invoking(x => x.RegisterServiceInto(null, typeof(object)))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("module");
		}

		[Theory, NancyAutoData]
		public void RegisterServiceInto_CalledWithNullServiceType_ExpectArgumentNullExceptionWithCorrectParamName(
			RouteRegistrar registrar, NancyModule module)
		{
			registrar.Invoking(x => x.RegisterServiceInto(module, null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceType");
		}

		[Theory, NancyAutoData]
		public void RegisterServicesInto_ArrayOverloadCalledWithNullModule_ExpectArgumentNullExceptionWithCorrectParamName(RouteRegistrar registrar)
		{
			registrar.Invoking(x => x.RegisterServicesInto(null, typeof(object), typeof(string)))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("module");
		}

		[Theory, NancyAutoData]
		public void RegisterServicesInto_ArrayOverloadCalledWithNullServiceTypes_ExpectArgumentNullExceptionWithCorrectParamName(
			RouteRegistrar registrar, NancyModule module)
		{
			registrar.Invoking(x => x.RegisterServicesInto(module, null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceTypes");
		}

		[Theory, NancyAutoData]
		public void RegisterServicesInto_EnumerableOverloadCalledWithNullModule_ExpectArgumentNullExceptionWithCorrectParamName(RouteRegistrar registrar)
		{
			registrar.Invoking(x => x.RegisterServicesInto(null, new[] {typeof(object), typeof(string)}.AsEnumerable()))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("module");
		}

		[Theory, NancyAutoData]
		public void RegisterServicesInto_EnumerableOverloadCalledWithNullServiceTypes_ExpectArgumentNullExceptionWithCorrectParamName(
			RouteRegistrar registrar, NancyModule module)
		{
			registrar.Invoking(x => x.RegisterServicesInto(module, (IEnumerable<Type>) null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceTypes");
		}

		[Theory, NancyAutoData]
		public void WithDispatchContext_CalledWithNullContext_ExpectArgumentNullExceptionWithCorrectParamName(RouteRegistrar registrar)
		{
			registrar.Invoking(x => x.WithDispatchContext(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("context");
		}

		[Theory, NancyAutoData]
		public void WithDispatchContext_Called_ExpectReturnedRegistrarIsNotTheSameInstanceAsCalledOn(RouteRegistrar registrar)
		{
			registrar.WithDispatchContext(x => x).Should().NotBeSameAs(registrar);
		}
	}
}
