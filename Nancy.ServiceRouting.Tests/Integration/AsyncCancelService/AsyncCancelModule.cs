using Nancy;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.AsyncCancelService
{
	public class AsyncCancelModule: NancyModule
	{
		public AsyncCancelModule(RouteRegistrar registrar)
		{
			registrar.RegisterServiceInto(this, typeof(AsyncCancelService));
		}
	}
}
