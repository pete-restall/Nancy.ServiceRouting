namespace Restall.Nancy.ServiceRouting.Tests.Integration.EchoService
{
	public class EchoService
	{
		public EchoResponse ServiceMethod(EchoUrlRequest request)
		{
			return new EchoResponse { TokenEcho = request.Token };
		}

		public EchoResponse ServiceMethod(EchoFormRequest request)
		{
			return new EchoResponse { TokenEcho = request.Token };
		}
	}
}
