using System;

namespace Nancy.ServiceRouting.Tests.Integration.EchoService
{
	[Route("/echo/{token}", "GET", "DELETE", "OPTIONS")]
	public class EchoUrlRequest
	{
		public Guid Token { get; private set; }
	}
}
