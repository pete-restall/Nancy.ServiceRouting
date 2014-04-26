using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.EchoService
{
	[Route("/echo/{token}", "GET", "DELETE", "OPTIONS")]
	public class EchoUrlRequest
	{
		public Guid Token { get; private set; }
	}
}
