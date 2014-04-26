namespace Restall.Nancy.ServiceRouting.Tests.Integration.CalculatorService
{
	public class AddService
	{
		public CalculatorResponse Add(AddRequest request)
		{
			return new CalculatorResponse { Result = request.Left + request.Right };
		}
	}
}
