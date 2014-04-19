namespace Nancy.ServiceRouting.Tests.Integration.CalculatorService
{
	[Route("/add/{left}/{right}", "GET", "DELETE", "OPTIONS")]
	[Route("/add", "PUT", "POST", "PATCH")]
	public class AddRequest
	{
		public AddRequest(): this(0, 0)
		{
		}

		public AddRequest(int left, int right)
		{
			this.Left = left;
			this.Right = right;
		}

		public int Left { get; set; }
		public int Right { get; set; }
	}
}
