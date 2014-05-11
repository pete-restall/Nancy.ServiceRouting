using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting.Sync
{
	public class SyncServiceMethodInvocation: IServiceMethodInvocation
	{
		public bool CanCreateInvocationDelegateFor(MethodInfo serviceMethod)
		{
			return !serviceMethod.IsAsyncCallable() && !serviceMethod.IsStatic && serviceMethod.NumberOfParameters() == 1;
		}

		public Delegate CreateInvocationDelegate(MethodInfo serviceMethod, ServiceMethodInvocationContext context)
		{
			if (!this.CanCreateInvocationDelegateFor(serviceMethod))
			{
				throw new ArgumentException(
					"Method " + serviceMethod + " should have a single parameter, not return a Task nor be marked async, and should not be static",
					"serviceMethod");
			}

			ParameterExpression request = Expression.Parameter(typeof(object), "request");
			if (serviceMethod.ReturnType == typeof(void))
			{
				return Expression.Lambda<Func<object, object>>(
					Expression.Block(
						SyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request),
						Expression.Constant(context.DefaultResponse, typeof(object))),
					request).Compile();
			}

			return Expression.Lambda<Func<object, object>>(
				SyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request),
				request).Compile();
		}
	}
}
