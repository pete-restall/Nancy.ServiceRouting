using Nancy;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.EchoService
{
	public class AsyncEchoModule: NancyModule
	{
		public AsyncEchoModule(RouteRegistrar registrar)
		{
			registrar.RegisterServiceInto(this, typeof(AsyncEchoService));
		}
	}
}
