using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Restall.Nancy.ServiceRouting.Async
{
	public class AsyncVoidServiceMethodInvocation: IServiceMethodInvocation
	{
		public bool CanCreateInvocationDelegateFor(MethodInfo serviceMethod)
		{
			return serviceMethod.ReturnType == typeof(void) && serviceMethod.IsAsyncDecorated() && !serviceMethod.IsStatic &&
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
			Action<object, CancellationToken> lambda = Expression.Lambda<Action<object, CancellationToken>>(
				serviceMethod.NumberOfParameters() == 1?
					AsyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request):
					AsyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request, cancel),
				request,
				cancel).Compile();

			return new Func<object, CancellationToken, Task<object>>((r, c) =>
				{
					lambda(r, c);
					return Task.FromResult(context.DefaultResponse);
				});
		}
	}
}
