using Restall.Nancy.ServiceRouting;

namespace Restall.Nancy.Demo.ServiceRouting.Api.Greet
{
	[Route("/greet/{name}", "GET")]
	[Route("/greet", "PUT")]
	public class GreetRequest
	{
		public string Name { get; set; }
	}
}
