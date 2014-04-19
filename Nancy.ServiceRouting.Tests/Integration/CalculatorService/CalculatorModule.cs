using System.Linq;

namespace Nancy.ServiceRouting.Tests.Integration.CalculatorService
{
	public class CalculatorModule: NancyModule
	{
		public CalculatorModule(RouteRegistrar routeRegistrar, bool useParamsRegistration)
		{
			if (useParamsRegistration)
				routeRegistrar.RegisterServicesInto(this, typeof(AddService), typeof(MultiplyService));
			else
				routeRegistrar.RegisterServicesInto(this, new[] {typeof(AddService), typeof(MultiplyService)}.AsEnumerable());
		}
	}
}
