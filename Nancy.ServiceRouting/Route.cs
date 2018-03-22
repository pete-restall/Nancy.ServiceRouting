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

		public string Name { get; }

		public string Verb { get; }

		public string Path { get; }

		public MethodInfo Method { get; }
	}
}
