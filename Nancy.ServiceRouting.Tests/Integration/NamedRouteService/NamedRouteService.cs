using System.Threading.Tasks;
using Nancy;
using Nancy.Linker;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.NamedRouteService
{
	public class NamedRouteService
	{
		private readonly NancyContext context;
		private readonly IResourceLinker linker;

		public NamedRouteService(NancyContext context, IResourceLinker linker)
		{
			this.context = context;
			this.linker = linker;
		}

		public NamedRouteResponse ResolveNamedRoute(ResolveNamedRouteRequest request)
		{
			return new NamedRouteResponse
				{
					Uri = this.linker.BuildRelativeUri(
						this.context,
						"SomeNamedRoute: " + request.RouteName,
						new { token = request.Token })
							.ToString()
				};
		}

		public object NamedRouteSyncServiceMethod(SyncNamedRouteRequest request)
		{
			return HttpStatusCode.NoContent;
		}

		public Task<object> NamedRouteAsyncServiceMethod(AsyncNamedRouteRequest request)
		{
			return Task.FromResult((object) HttpStatusCode.NoContent);
		}
	}
}
