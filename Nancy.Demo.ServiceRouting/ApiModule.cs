using System.Linq;
using Nancy;
using Restall.Nancy.ServiceRouting;

namespace Restall.Nancy.Demo.ServiceRouting
{
	public class ApiModule: NancyModule
	{
		public ApiModule(RouteRegistrar routes)
		{
			routes.RegisterServicesInto(
				this,
				AllTypes.InAssembly.Where(
					x => x.Namespace.StartsWith("Restall.Nancy.Demo.ServiceRouting.Api") && x.Name.EndsWith("Service")));
		}
	}
}
