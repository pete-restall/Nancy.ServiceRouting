using System;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public interface IServiceMethodInvocation
	{
		bool CanCreateInvocationDelegateFor(MethodInfo serviceMethod);

		Delegate CreateInvocationDelegate(
			Func<object> serviceFactory,
			Func<object, object> requestBinder,
			MethodInfo serviceMethod,
			object defaultResponse);
	}
}
