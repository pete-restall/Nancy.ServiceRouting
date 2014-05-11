using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.AsyncCancelService
{
	[Route("/cancel/long-running", "PUT", "POST", "PATCH")]
	public class LongRunningFormRequest
	{
		public LongRunningFormRequest(): this(0, Guid.Empty)
		{
		}

		public LongRunningFormRequest(int timeoutSeconds, Guid token)
		{
			this.TimeoutSeconds = timeoutSeconds;
			this.Token = token;
		}

		public int TimeoutSeconds { get; private set; }
		public Guid Token { get; private set; }
	}
}
