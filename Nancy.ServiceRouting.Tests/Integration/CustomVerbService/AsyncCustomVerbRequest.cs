using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.CustomVerbService
{
	[Route("/some-async-route/{token}", "FOO")]
	public class AsyncCustomVerbRequest
	{
		public Guid Token { get; private set; }
	}
}
