using System.Linq;
using Nancy.ServiceRouting;

namespace Nancy.Demo.ServiceRouting
{
	public class ApiModule: NancyModule
	{
		public ApiModule(RouteRegistrar routes)
		{
			routes.RegisterServicesInto(
				this, AllTypes.InAssembly.Where(x => x.Namespace.StartsWith("Nancy.Demo.ServiceRouting.Api") && x.Name.EndsWith("Service")));
		}
	}
}
