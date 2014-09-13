using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class RouteTableTest
	{
		[Fact]
		public void Constructor_CalledWithNullServiceRoutes_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new RouteTable(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("routes");
		}

		[Theory, NancyAutoData]
		public void GetRoutesForAllVerbs_Called_ExpectSameRoutesAsPassedToConstructor(Route[] routes)
		{
			new RouteTable(routes).GetRoutesForAllVerbs().ShouldBeEquivalentTo(routes);
		}

		[Theory, NancyAutoData]
		public void GetRoutesForAllVerbs_Called_ExpectReferenceIsNotSameAsPassedToConstructor(Route[] routes)
		{
			((object) new RouteTable(routes).GetRoutesForAllVerbs()).Should().NotBeSameAs(routes);
		}

		[Theory, NancyAutoData]
		public void GetRoutesForVerb_CalledWithNullVerb_ExpectArgumentNullExceptionWithCorrectParamName(Route[] routes)
		{
			new RouteTable(routes).Invoking(x => x.GetRoutesForVerb(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("verb");
		}

		[Theory, NancyAutoData]
		public void GetRoutesForVerb_Called_ExpectOnlyRoutesMatchingVerbAreReturned(
			Route[] mixedVerbRoutes, string verb, string[] routes, MethodInfo method)
		{
			var specificVerbRoutes = routes.Select(x => new Route(verb, x, method)).ToArray();
			var table = new RouteTable(mixedVerbRoutes.Concat(specificVerbRoutes).Shuffle());
			table.GetRoutesForVerb(verb).Should().BeEquivalentTo(specificVerbRoutes);
		}
	}
}
