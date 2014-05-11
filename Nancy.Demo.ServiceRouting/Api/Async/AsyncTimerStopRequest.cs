using System;
using Restall.Nancy.ServiceRouting;

namespace Restall.Nancy.Demo.ServiceRouting.Api.Async
{
	[Route("/async/timer/{timerId:guid}/stop")]
	public class AsyncTimerStopRequest
	{
		public Guid TimerId { get; private set; }
	}
}
