using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace Nancy.ServiceRouting.Tests.Unit
{
	public static class ServiceRouteEnumerableAssertionExtensions
	{
		public static void ShouldBeEquivalentTo(this IEnumerable<ServiceRoute> actualRoutes, params ServiceRoute[] expectedRoutes)
		{
			actualRoutes.ShouldBeEquivalentTo(expectedRoutes.AsEnumerable());
		}

		public static void ShouldBeEquivalentTo(this IEnumerable<ServiceRoute> actualRoutes, IEnumerable<ServiceRoute> expectedRoutes)
		{
			actualRoutes.ToArray().ShouldBeEquivalentTo(expectedRoutes.ToArray());
		}

		public static void ShouldBeEquivalentTo(this ServiceRoute[] actualRoutes, ServiceRoute[] expectedRoutes)
		{
			actualRoutes.Length.Should().Be(expectedRoutes.Length, " collections should be equivalent");
			foreach (var expectedRoute in expectedRoutes)
			{
				var y = expectedRoute;
				actualRoutes.Should().Contain(x => AreRoutesEqual(x, y));
			}
		}

		private static bool AreRoutesEqual(ServiceRoute x, ServiceRoute y)
		{
			return x.Verb == y.Verb && x.Path == y.Path && x.Method.MetadataToken == y.Method.MetadataToken;
		}
	}
}
