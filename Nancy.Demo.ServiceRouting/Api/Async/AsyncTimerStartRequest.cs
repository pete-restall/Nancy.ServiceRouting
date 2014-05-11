using System;
using Restall.Nancy.ServiceRouting;

namespace Restall.Nancy.Demo.ServiceRouting.Api.Async
{
	[Route("/async/timer/{timerId:guid}/start")]
	public class AsyncTimerStartRequest
	{
		public Guid TimerId { get; private set; }
	}
}
