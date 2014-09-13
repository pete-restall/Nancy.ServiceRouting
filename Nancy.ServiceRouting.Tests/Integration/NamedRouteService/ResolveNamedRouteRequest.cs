using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.NamedRouteService
{
	[Route("/named/{routeName}/{token}")]
	public class ResolveNamedRouteRequest
	{
		public string RouteName { get; private set; }
		public Guid Token { get; private set; }
	}
}
