using System.Linq;
using Nancy;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.CalculatorService
{
	public class AsyncCalculatorModule: NancyModule
	{
		public AsyncCalculatorModule(RouteRegistrar routeRegistrar, bool useParamsRegistration)
		{
			if (useParamsRegistration)
				routeRegistrar.RegisterServicesInto(this, typeof(AsyncAddService), typeof(AsyncMultiplyService));
			else
				routeRegistrar.RegisterServicesInto(this, new[] {typeof(AsyncAddService), typeof(AsyncMultiplyService)}.AsEnumerable());
		}
	}
}
