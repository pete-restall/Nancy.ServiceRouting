using System.Collections.Generic;
using System.Linq;

namespace Restall.Nancy.ServiceRouting
{
	public class RouteTable
	{
		private readonly Route[] routes;

		public RouteTable(IEnumerable<Route> routes)
		{
			this.routes = routes.ToArray();
		}

		public IEnumerable<Route> GetRoutesForAllVerbs()
		{
			return this.routes;
		}

		public IEnumerable<Route> GetRoutesForVerb(string verb)
		{
			return this.routes.Where(x => x.Verb == verb);
		}
	}
}
