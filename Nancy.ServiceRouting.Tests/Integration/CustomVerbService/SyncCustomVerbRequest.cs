using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.CustomVerbService
{
	[Route("/some-sync-route/{token}", "WHATEVER")]
	public class SyncCustomVerbRequest
	{
		public Guid Token { get; private set; }
	}
}
