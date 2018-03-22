using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple=true, Inherited=true)]
	public class RouteAttribute: Attribute
	{
		public RouteAttribute(string path, params string[] verbs)
		{
			this.Path = path;
			this.Verbs = verbs.Length > 0? verbs.Select(x => x.Trim().ToUpper()).ToArray(): new[] {"GET"};
		}

		public string Path { get; }

		public IEnumerable<string> Verbs { get; }

		public virtual IEnumerable<Route> WireToMethod(MethodInfo method)
		{
			return this.Verbs.Select(v => new Route(v, this.Path, method));
		}
	}
}
