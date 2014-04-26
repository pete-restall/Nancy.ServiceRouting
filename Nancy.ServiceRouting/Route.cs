using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public class Route
	{
		public Route(string verb, string path, MethodInfo method)
		{
			this.Verb = verb;
			this.Path = path;
			this.Method = method;
		}

		public string Verb { get; private set; }
		public string Path { get; private set; }
		public MethodInfo Method { get; private set; }
	}
}
