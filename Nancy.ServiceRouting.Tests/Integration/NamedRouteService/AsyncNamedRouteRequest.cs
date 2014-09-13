using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.NamedRouteService
{
	[NamedRoute("SomeNamedRoute: async", "/some-named-async-route/{token}")]
	public class AsyncNamedRouteRequest
	{
		public Guid Token { get; private set; }
	}
}
