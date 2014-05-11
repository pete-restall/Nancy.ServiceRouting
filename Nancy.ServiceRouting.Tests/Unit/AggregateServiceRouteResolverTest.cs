using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class AggregateServiceRouteResolverTest
	{
		[Fact]
		public void Constructor_CalledWithNullEnumerableServiceRouteResolvers_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateServiceRouteResolver((IEnumerable<IServiceRouteResolver>) null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceRouteResolvers");
		}

		[Fact]
		public void Constructor_CalledWithNullParamsServiceRouteResolvers_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateServiceRouteResolver(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceRouteResolvers");
		}

		[Theory, NancyAutoData]
		public void GetServiceRoutes_CalledWithNullServiceType_ExpectArgumentNullExceptionWithCorrectParamName(
			[WithinInclusiveRange(1, 10)] int numberOfInnerResolvers)
		{
			var resolver = new AggregateServiceRouteResolver(
				numberOfInnerResolvers.Select(x => MockRepository.GenerateStub<IServiceRouteResolver>()));

			resolver.Invoking(x => x.GetServiceRoutes(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceType");
		}

		[Theory, NancyAutoData]
		public void GetServiceRoutes_CalledWithNoInnerResolvers_ExpectEmptyEnumerableIsReturned(Type serviceType)
		{
			new AggregateServiceRouteResolver(new IServiceRouteResolver[0])
				.GetServiceRoutes(serviceType).Should().BeEmpty();
		}

		[Theory, NancyAutoData]
		public void GetServiceRoutes_Called_ExpectReturnedEnumerableContainsAllRoutesFromInnerResolvers(
			[WithinInclusiveRange(1, 10)] int numberOfInnerResolvers, Type serviceType)
		{
			var stubs = numberOfInnerResolvers.Select(x => new
				{
					Resolver = MockRepository.GenerateStub<IServiceRouteResolver>(),
					Routes = CreateAnyNumberOfDummyRoutes()
				}).ToArray();

			stubs.ForEach(s => s.Resolver.Stub(x => x.GetServiceRoutes(Arg<Type>.Is.Equal(serviceType))).Return(s.Routes));

			new AggregateServiceRouteResolver(stubs.Select(x => x.Resolver)).GetServiceRoutes(serviceType)
				.Should().BeEquivalentTo(stubs.SelectMany(x => x.Routes));
		}

		private static Route[] CreateAnyNumberOfDummyRoutes()
		{
			int numberOfRoutes = RandomNumbers.Next(0, 10);
			return numberOfRoutes.Select(x => CreateDummyRoute()).ToArray();
		}

		private static Route CreateDummyRoute()
		{
			return new Route(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), InfoOf.Method<object>(x => x.ToString()));
		}

		[Theory, NancyAutoData]
		public void GetServiceRoutes_CalledMultipleTimes_ExpectEnumerableOfInnerResolversIsOnlyEnumeratedOnce(
			Type serviceType, Type anotherServiceType)
		{
			var innerResolvers = Mock.Enumerable<IServiceRouteResolver>();
			var resolver = new AggregateServiceRouteResolver(innerResolvers);
			resolver.GetServiceRoutes(serviceType).ForEach(x => { });
			resolver.GetServiceRoutes(anotherServiceType).ForEach(x => { });

			innerResolvers.AssertWasCalled(x => x.GetEnumerator(), x => x.Repeat.Once());
		}
	}
}
