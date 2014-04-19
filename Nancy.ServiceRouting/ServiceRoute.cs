using System.Reflection;

namespace Nancy.ServiceRouting
{
	public class ServiceRoute
	{
		public ServiceRoute(string verb, string path, MethodInfo method)
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
