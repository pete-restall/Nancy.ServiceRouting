using Nancy;
using Nancy.Linker;

namespace Restall.Nancy.Demo.ServiceRouting.Api
{
	public class IndexService
	{
		private readonly NancyContext context;
		private readonly IResourceLinker linker;

		public IndexService(NancyContext context, IResourceLinker linker)
		{
			this.context = context;
			this.linker = linker;
		}

		public IndexResponse Welcome(IndexRequest request)
		{
			return new IndexResponse
				{
					AvailableDemos = new[]
						{
							new AvailableDemo { Href = this.HrefForRouteNamed("GreetIndex"), Title = "Simple Greeting Request / Response" },
							new AvailableDemo { Href = this.HrefForRouteNamed("AsyncIndex"), Title = "Async Functionality" }
						}
				};
		}

		private string HrefForRouteNamed(string name)
		{
			return this.linker.BuildRelativeUri(this.context, name).ToString();
		}
	}
}
