using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public class NamedRouteAttribute: RouteAttribute
	{
		public NamedRouteAttribute(string name, string path, params string[] verbs): base(path, verbs)
		{
			this.Name = name.Trim();
			if (this.Name == string.Empty)
				throw new ArgumentException("Route Name cannot be empty or whitespace", nameof(name));
		}

		public override IEnumerable<Route> WireToMethod(MethodInfo method)
		{
			return this.Verbs.Select(v => new Route(this.Name, v, this.Path, method));
		}

		public string Name { get; }
	}
}
