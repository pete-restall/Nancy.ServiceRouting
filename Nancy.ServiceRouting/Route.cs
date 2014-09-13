using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public class Route
	{
		public Route(string verb, string path, MethodInfo method): this(string.Empty, verb, path, method)
		{
		}

		public Route(string name, string verb, string path, MethodInfo method)
		{
			this.Name = name;
			this.Verb = verb;
			this.Path = path;
			this.Method = method;
		}

		public string Name { get; private set; }

		public string Verb { get; private set; }

		public string Path { get; private set; }

		public MethodInfo Method { get; private set; }
	}
}
