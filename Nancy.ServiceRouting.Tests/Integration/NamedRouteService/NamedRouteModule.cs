using Nancy;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.NamedRouteService
{
	public class NamedRouteModule: NancyModule
	{
		public NamedRouteModule(RouteRegistrar registrar)
		{
			registrar.RegisterServiceInto(this, typeof(NamedRouteService));
		}
	}
}
