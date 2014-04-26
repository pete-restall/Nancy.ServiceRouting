namespace Restall.Nancy.Demo.ServiceRouting.Api.Greet
{
	public class GreetService
	{
		public object Welcome(GreetIndex request)
		{
			return new GreetIndex();
		}

		public object Greet(GreetRequest request)
		{
			return string.IsNullOrWhiteSpace(request.Name)?
				this.Welcome(new GreetIndex()):
				new Greeting { Text = "Hello, " + request.Name + " !" };
		}
	}
}
