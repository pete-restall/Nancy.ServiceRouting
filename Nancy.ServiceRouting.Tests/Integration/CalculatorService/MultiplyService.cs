namespace Nancy.ServiceRouting.Tests.Integration.CalculatorService
{
	public class MultiplyService
	{
		public CalculatorResponse Multiply(MultiplyRequest request)
		{
			return new CalculatorResponse { Result = request.Left * request.Right };
		}
	}
}
