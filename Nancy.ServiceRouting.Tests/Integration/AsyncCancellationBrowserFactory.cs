using System.Threading;
using Nancy.Testing;
using Restall.Nancy.ServiceRouting.Tests.Integration.AsyncCancelService;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public static class AsyncCancellationBrowserFactory
	{
		public static Browser CreateBrowserWithCancellationToken(RouteRegistrar registrar, CancellationToken cancel)
		{
			var bootstrapper = new ConfigurableBootstrapper(with => with
				.Module(new AsyncCancelModule(registrar))
				.NancyEngine<NancyEngineWithAsyncCancellation>());

			var browser = new Browser(bootstrapper);
			((NancyEngineWithAsyncCancellation) bootstrapper.GetEngine()).CancellationToken = cancel;
			return browser;
		}
	}
}
