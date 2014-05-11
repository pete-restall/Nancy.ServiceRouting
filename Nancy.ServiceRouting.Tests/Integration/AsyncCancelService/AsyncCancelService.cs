using System;
using System.Threading;
using System.Threading.Tasks;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.AsyncCancelService
{
	public class AsyncCancelService
	{
		public async Task<CancelResponse> LongRunningService(LongRunningUrlRequest request, CancellationToken cancel)
		{
			await Task.FromResult(cancel.WaitHandle.WaitOne(TimeSpan.FromSeconds(request.TimeoutSeconds)));
			return new CancelResponse { TokenEcho = request.Token, WasCancelled = cancel.IsCancellationRequested };
		}

		public async Task<CancelResponse> LongRunningService(LongRunningFormRequest request, CancellationToken cancel)
		{
			await Task.FromResult(cancel.WaitHandle.WaitOne(TimeSpan.FromSeconds(request.TimeoutSeconds)));
			return new CancelResponse { TokenEcho = request.Token, WasCancelled = cancel.IsCancellationRequested };
		}
	}
}
