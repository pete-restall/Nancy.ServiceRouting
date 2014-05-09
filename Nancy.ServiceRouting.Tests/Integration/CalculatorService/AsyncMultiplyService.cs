using System.Threading.Tasks;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.CalculatorService
{
	public class AsyncMultiplyService
	{
		public async Task<CalculatorResponse> Multiply(MultiplyRequest request)
		{
			await LongRunningTask.Instance();
			return new CalculatorResponse { Result = request.Left * request.Right };
		}
	}
}
