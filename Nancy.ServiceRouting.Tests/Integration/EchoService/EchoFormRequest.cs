using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.EchoService
{
	[Route("/echo", "PUT", "POST", "PATCH")]
	public class EchoFormRequest
	{
		public Guid Token { get; private set; }
	}
}
