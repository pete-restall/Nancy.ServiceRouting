using System;

namespace Restall.Nancy.Demo.ServiceRouting.Api.Async
{
	public class AsyncTimerResponse
	{
		public AsyncTimerResponse(Guid id, TimeSpan runningTime, int startThreadId, int endThreadId)
		{
			this.Id = id;
			this.Minutes = runningTime.Minutes;
			this.Seconds = runningTime.Seconds;
			this.Milliseconds = runningTime.Milliseconds;
			this.StartedOnThread = startThreadId;
			this.EndedOnThread = endThreadId;
		}

		public Guid Id { get; private set; }
		public int Minutes { get; private set; }
		public int Seconds { get; private set; }
		public int Milliseconds { get; private set; }
		public int StartedOnThread { get; private set; }
		public int EndedOnThread { get; private set; }
	}
}
