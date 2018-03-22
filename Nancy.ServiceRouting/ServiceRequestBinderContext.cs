using Nancy;

namespace Restall.Nancy.ServiceRouting
{
	public class ServiceRequestBinderContext
	{
		public ServiceRequestBinderContext(NancyModule nancyModule)
		{
			this.NancyModule = nancyModule;
		}

		public NancyModule NancyModule { get; }
	}
}
