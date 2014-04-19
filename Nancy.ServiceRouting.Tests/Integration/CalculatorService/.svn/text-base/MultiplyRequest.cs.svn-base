namespace Nancy.ServiceRouting.Tests.Integration.CalculatorService
{
	[Route("/multiply/{left}/{right}", "GET", "DELETE", "OPTIONS")]
	[Route("/multiply", "PUT", "POST", "PATCH")]
	public class MultiplyRequest
	{
		public MultiplyRequest(): this(0, 0)
		{
		}

		public MultiplyRequest(int left, int right)
		{
			this.Left = left;
			this.Right = right;
		}

		public int Left { get; set; }
		public int Right { get; set; }
	}
}
