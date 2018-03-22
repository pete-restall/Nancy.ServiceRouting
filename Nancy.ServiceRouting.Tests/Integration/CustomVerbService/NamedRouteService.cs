using System.Threading.Tasks;
using Nancy;
using Nancy.Linker;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.CustomVerbService
{
	public class CustomVerbService
	{
		private readonly NancyContext context;
		private readonly IResourceLinker linker;

		public CustomVerbService(NancyContext context, IResourceLinker linker)
		{
			this.context = context;
			this.linker = linker;
		}

		public CustomVerbResponse ResolveCustomVerb(SyncCustomVerbRequest request)
		{
			return new CustomVerbResponse { EchoToken = request.Token };
		}

		public Task<CustomVerbResponse> CustomVerbAsyncServiceMethod(AsyncCustomVerbRequest request)
		{
			return Task.FromResult(new CustomVerbResponse { EchoToken = request.Token });
		}
	}
}
