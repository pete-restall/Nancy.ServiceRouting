using System;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.AsyncCancelService
{
	public class CancelResponse
	{
		public Guid TokenEcho { get; set; }

		public bool WasCancelled { get; set; }
	}
}
