using System.Collections.Generic;
using System.Linq;

namespace Nancy.ServiceRouting
{
	public class RouteTable
	{
		private readonly ServiceRoute[] serviceRoutes;

		public RouteTable(IEnumerable<ServiceRoute> serviceRoutes)
		{
			this.serviceRoutes = serviceRoutes.ToArray();
		}

		public IEnumerable<ServiceRoute> GetRoutesForAllVerbs()
		{
			return this.serviceRoutes;
		}

		public IEnumerable<ServiceRoute> GetRoutesForVerb(string verb)
		{
			return this.serviceRoutes.Where(x => x.Verb == verb);
		}
	}
}
