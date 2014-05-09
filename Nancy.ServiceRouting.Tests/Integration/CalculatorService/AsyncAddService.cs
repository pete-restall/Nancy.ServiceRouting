using System.Threading.Tasks;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.CalculatorService
{
	public class AsyncAddService
	{
		public async Task<CalculatorResponse> Add(AddRequest request)
		{
			await LongRunningTask.Instance();
			return new CalculatorResponse { Result = request.Left + request.Right };
		}
	}
}
