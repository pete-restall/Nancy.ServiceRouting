using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Restall.Nancy.Demo.ServiceRouting.Api.Async
{
	public class AsyncService
	{
		private readonly static IDictionary<Guid, ManualResetEvent> Timers = new ConcurrentDictionary<Guid, ManualResetEvent>();

		public object Welcome(AsyncIndex request)
		{
			return new AsyncIndex();
		}

		public async Task<AsyncTimerResponse> StartTimer(AsyncTimerStartRequest request)
		{
			ManualResetEvent flag = new ManualResetEvent(false);
			Timers.Add(request.TimerId, flag);

			int startThreadId = Thread.CurrentThread.ManagedThreadId;
			Stopwatch stopwatch = Stopwatch.StartNew();
			await Task.Factory.StartNew(() => flag.WaitOne(TimeSpan.FromMinutes(1)));
			stopwatch.Stop();
			int endThreadId = Thread.CurrentThread.ManagedThreadId;

			Timers.Remove(request.TimerId);
			return new AsyncTimerResponse(request.TimerId, stopwatch.Elapsed, startThreadId, endThreadId);
		}

		public void StopTimer(AsyncTimerStopRequest request)
		{
			ManualResetEvent flag;
			if (Timers.TryGetValue(request.TimerId, out flag))
				flag.Set();
		}
	}
}
