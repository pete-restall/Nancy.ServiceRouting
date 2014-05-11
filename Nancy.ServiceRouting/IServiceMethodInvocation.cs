using System;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public interface IServiceMethodInvocation
	{
		bool CanCreateInvocationDelegateFor(MethodInfo serviceMethod);
		Delegate CreateInvocationDelegate(MethodInfo serviceMethod, ServiceMethodInvocationContext context);
	}
}
