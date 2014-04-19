using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Nancy.ServiceRouting.Tests.Unit
{
	public class RouteTableTest
	{
		[Fact]
		public void Constructor_CalledWithNullServiceRoutes_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new RouteTable(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceRoutes");
		}

		[Theory, NancyAutoData]
		public void GetRoutesForAllVerbs_Called_ExpectSameRoutesAsPassedToConstructor(ServiceRoute[] serviceRoutes)
		{
			new RouteTable(serviceRoutes).GetRoutesForAllVerbs().ShouldBeEquivalentTo(serviceRoutes);
		}

		[Theory, NancyAutoData]
		public void GetRoutesForAllVerbs_Called_ExpectReferenceIsNotSameAsPassedToConstructor(ServiceRoute[] serviceRoutes)
		{
			((object) new RouteTable(serviceRoutes).GetRoutesForAllVerbs()).Should().NotBeSameAs(serviceRoutes);
		}

		[Theory, NancyAutoData]
		public void GetRoutesForVerb_CalledWithNullVerb_ExpectArgumentNullExceptionWithCorrectParamName(ServiceRoute[] serviceRoutes)
		{
			new RouteTable(serviceRoutes).Invoking(x => x.GetRoutesForVerb(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("verb");
		}

		[Theory, NancyAutoData]
		public void GetRoutesForVerb_Called_ExpectOnlyRoutesMatchingVerbAreReturned(
			ServiceRoute[] mixedVerbRoutes, string verb, string[] routes)
		{
			var specificVerbRoutes = routes.Select(x => new ServiceRoute(verb, x, DummyMethodInfo)).ToArray();
			var table = new RouteTable(mixedVerbRoutes.Concat(specificVerbRoutes).Shuffle());
			table.GetRoutesForVerb(verb).Should().BeEquivalentTo(specificVerbRoutes);
		}

		private static MethodInfo DummyMethodInfo
		{
			get { return InfoOf.Method<object>(x => x.ToString()); }
		}
	}
}
