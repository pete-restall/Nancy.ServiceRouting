using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Rhino.Mocks;
using Xunit;

namespace Nancy.ServiceRouting.Tests.Unit
{
	public class RouteTableBuilderTest
	{
		private class StubService
		{
			public void Method1() { }
			public void Method2() { }
			public void Method3() { }
		}

		[Fact]
		public void Constructor_CalledWithServiceRouteResolver_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new RouteTableBuilder(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceRouteResolver");
		}

		[Fact]
		public void ForService_CalledWithNullServiceType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new RouteTableBuilder(MockRepository.GenerateStub<IServiceRouteResolver>()).Invoking(x => x.ForService(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceType");
		}

		[Fact]
		public void ForService_Called_ExpectSameBuilderInstanceIsReturned()
		{
			var serviceRouteResolver = MockRepository.GenerateStub<IServiceRouteResolver>();
			serviceRouteResolver.Stub(x => x.GetServiceRoutes(Arg<Type>.Is.Anything)).Return(new ServiceRoute[0]);

			var builder = new RouteTableBuilder(serviceRouteResolver);
			builder.ForService(typeof(StubService)).Should().BeSameAs(builder);
		}

		[Fact]
		public void Build_Called_ExpectRouteTableIsConstructedWithResolvedServiceMethods()
		{
			var serviceRoutes = typeof(StubService).GetMethods().Select(StubServiceRouteForMethod).ToArray();
			var serviceRouteResolver = MockRepository.GenerateStub<IServiceRouteResolver>();
			serviceRouteResolver.Stub(x => x.GetServiceRoutes(Arg<Type>.Is.Equal(typeof(StubService)))).Return(serviceRoutes);

			new RouteTableBuilder(serviceRouteResolver)
				.ForService(typeof(StubService))
				.Build()
				.GetRoutesForAllVerbs()
				.ShouldBeEquivalentTo(serviceRoutes);
		}

		private static ServiceRoute StubServiceRouteForMethod(MethodInfo method)
		{
			return new ServiceRoute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), method);
		}
	}
}
