using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public static class ServiceRouteEnumerableAssertionExtensions
	{
		public static void ShouldBeEquivalentTo(this IEnumerable<Route> actualRoutes, params Route[] expectedRoutes)
		{
			actualRoutes.ShouldBeEquivalentTo(expectedRoutes.AsEnumerable());
		}

		public static void ShouldBeEquivalentTo(this IEnumerable<Route> actualRoutes, IEnumerable<Route> expectedRoutes)
		{
			actualRoutes.ToArray().ShouldBeEquivalentTo(expectedRoutes.ToArray());
		}

		private static void ShouldBeEquivalentTo(this Route[] actualRoutes, Route[] expectedRoutes)
		{
			actualRoutes.Length.Should().Be(expectedRoutes.Length, " collections should be equivalent");
			foreach (var expectedRoute in expectedRoutes)
			{
				var y = expectedRoute;
				actualRoutes.Should().Contain(x => AreRoutesEqual(x, y));
			}
		}

		private static bool AreRoutesEqual(Route x, Route y)
		{
			return x.Name == y.Name && x.Verb == y.Verb && x.Path == y.Path && x.Method.MetadataToken == y.Method.MetadataToken;
		}
	}
}
