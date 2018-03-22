using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Restall.Nancy.ServiceRouting.Async
{
	public class AsyncTaskServiceMethodInvocation: IServiceMethodInvocation
	{
		public bool CanCreateInvocationDelegateFor(MethodInfo serviceMethod)
		{
			return
				serviceMethod.ReturnType == typeof(Task) && serviceMethod.IsAsyncCallable() && !serviceMethod.IsStatic &&
				(serviceMethod.NumberOfParameters() == 1 ||
				(serviceMethod.NumberOfParameters() == 2 && serviceMethod.TypeOfSecondParameter() == typeof(CancellationToken)));
		}

		public Delegate CreateInvocationDelegate(MethodInfo serviceMethod, ServiceMethodInvocationContext context)
		{
			if (!this.CanCreateInvocationDelegateFor(serviceMethod))
			{
				throw new ArgumentException(
					"Method " + serviceMethod + " should have a single parameter, be marked as async or return a Task, and should not be static",
					nameof(serviceMethod));
			}

			ParameterExpression request = Expression.Parameter(typeof(object), "request");
			ParameterExpression cancel = Expression.Parameter(typeof(CancellationToken), "cancel");
			Func<object, CancellationToken, Task> lambda = Expression.Lambda<Func<object, CancellationToken, Task>>(
				serviceMethod.NumberOfParameters() == 1?
					AsyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request):
					AsyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request, cancel),
				request,
				cancel).Compile();

			return new Func<object, CancellationToken, Task<object>>(async (r, c) =>
				{
					await lambda(r, c);
					return context.DefaultResponse;
				});
		}
	}
}
