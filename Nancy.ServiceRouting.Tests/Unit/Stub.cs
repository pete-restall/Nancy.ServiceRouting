using System;
using Rhino.Mocks;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class Stub
	{
		public static ServiceMethodInvocationContext InvocationContextFor<TService, TDto>(
			TService service, object request, TDto dto, object defaultResponse = null)
		{
			return new ServiceMethodInvocationContext(
				() => service,
				StubRequestBinderFor(request, dto),
				defaultResponse);
		}

		private static Func<object, object> StubRequestBinderFor<TDto>(object request, TDto dto)
		{
			var requestBinder = MockRepository.GenerateStub<Func<object, object>>();
			requestBinder.Stub(x => x(Arg<object>.Is.Same(request))).Return(dto);
			return requestBinder;
		}
	}
}
