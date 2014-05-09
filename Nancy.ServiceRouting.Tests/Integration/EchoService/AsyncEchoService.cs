using System.Threading.Tasks;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.EchoService
{
	public class AsyncEchoService
	{
		public async Task<EchoResponse> ServiceMethod(EchoUrlRequest request)
		{
			await LongRunningTask.Instance();
			return new EchoResponse { TokenEcho = request.Token };
		}

		public async Task<EchoResponse> ServiceMethod(EchoFormRequest request)
		{
			await LongRunningTask.Instance();
			return new EchoResponse { TokenEcho = request.Token };
		}
	}
}
