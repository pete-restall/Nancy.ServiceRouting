using Nancy;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.CustomVerbService
{
	public class CustomVerbModule: NancyModule
	{
		public CustomVerbModule(RouteRegistrar registrar)
		{
			registrar.RegisterServiceInto(this, typeof(CustomVerbService));
		}
	}
}
