using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.NamedRouteService
{
	[NamedRoute("SomeNamedRoute: sync", "/some-named-sync-route/{token}")]
	public class SyncNamedRouteRequest
	{
		public Guid Token { get; private set; }
	}
}
