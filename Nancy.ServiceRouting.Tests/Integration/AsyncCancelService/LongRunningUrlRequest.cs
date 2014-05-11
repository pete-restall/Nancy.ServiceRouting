using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.AsyncCancelService
{
	[Route("/cancel/long-running/{timeoutSeconds}/{token}", "GET", "DELETE", "OPTIONS")]
	public class LongRunningUrlRequest
	{
		public int TimeoutSeconds { get; private set; }
		public Guid Token { get; private set; }
	}
}
