using System;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public interface IServiceMethodInvocation
	{
		Func<object, object> CreateInvocationDelegate(Func<object> serviceFactory, MethodInfo serviceMethod, object defaultResponse);
	}
}
