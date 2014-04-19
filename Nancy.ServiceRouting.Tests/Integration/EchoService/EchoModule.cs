namespace Nancy.ServiceRouting.Tests.Integration.EchoService
{
	public class EchoModule: NancyModule
	{
		public EchoModule(RouteRegistrar registrar)
		{
			registrar.RegisterServiceInto(this, typeof(EchoService));
		}
	}
}
