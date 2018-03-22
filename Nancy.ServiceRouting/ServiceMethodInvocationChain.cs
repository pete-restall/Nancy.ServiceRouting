using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	public class ServiceMethodInvocationChain: IServiceMethodInvocation
	{
		private readonly IServiceMethodInvocation[] serviceMethodInvocations;

		public ServiceMethodInvocationChain(IEnumerable<IServiceMethodInvocation> serviceMethodInvocations):
			this(serviceMethodInvocations.ToArray())
		{
		}

		public ServiceMethodInvocationChain(params IServiceMethodInvocation[] serviceMethodInvocations)
		{
			this.serviceMethodInvocations = serviceMethodInvocations;
		}

		public bool CanCreateInvocationDelegateFor(MethodInfo serviceMethod)
		{
			return this.serviceMethodInvocations.Any(x => x.CanCreateInvocationDelegateFor(serviceMethod));
		}

		public Delegate CreateInvocationDelegate(MethodInfo serviceMethod, ServiceMethodInvocationContext context)
		{
			IServiceMethodInvocation invocation = this.serviceMethodInvocations.FirstOrDefault(x => x.CanCreateInvocationDelegateFor(serviceMethod));
			if (invocation == null)
			{
				throw new ArgumentException(
					"Method " + serviceMethod + " cannot be invoked as a Service Method as its signature is unknown",
					nameof(serviceMethod));
			}

			return invocation.CreateInvocationDelegate(serviceMethod, context);
		}
	}
}
