using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Restall.Nancy.ServiceRouting
{
	public class AsyncTaskOfTServiceMethodInvocation: IServiceMethodInvocation
	{
		public bool CanCreateInvocationDelegateFor(MethodInfo serviceMethod)
		{
			return
				serviceMethod.ReturnType.IsGenericType && serviceMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) &&
				serviceMethod.IsAsyncCallable() && !serviceMethod.IsStatic &&
				(serviceMethod.NumberOfParameters() == 1 ||
				(serviceMethod.NumberOfParameters() == 2 && serviceMethod.TypeOfSecondParameter() == typeof(CancellationToken)));
		}

		public Delegate CreateInvocationDelegate(MethodInfo serviceMethod, ServiceMethodInvocationContext context)
		{
			if (!this.CanCreateInvocationDelegateFor(serviceMethod))
			{
				throw new ArgumentException(
					"Method " + serviceMethod + " should have a single parameter, be marked as async or return a Task, and should not be static",
					"serviceMethod");
			}

			MethodInfo delegateCreator = CreateSpecialisedDelegateCreator(serviceMethod);
			return (Delegate) delegateCreator.Invoke(null, new object[] {serviceMethod, context});
		}

		private static MethodInfo CreateSpecialisedDelegateCreator(MethodInfo serviceMethod)
		{
			MethodInfo genericDelegateCreator =
				InfoOf.Method<AsyncVoidServiceMethodInvocation>(x => CreateInvocationDelegate<object>(null, null))
					.GetGenericMethodDefinition();

			return genericDelegateCreator.MakeGenericMethod(GetTaskResultTypeFromMethodReturnType(serviceMethod));
		}

		private static Func<object, CancellationToken, Task<object>> CreateInvocationDelegate<T>(
			MethodInfo serviceMethod, ServiceMethodInvocationContext context)
		{
			ParameterExpression request = Expression.Parameter(typeof(object), "request");
			ParameterExpression cancel = Expression.Parameter(typeof(CancellationToken), "cancel");
			Func<object, CancellationToken, Task<T>> lambda = Expression.Lambda<Func<object, CancellationToken, Task<T>>>(
				serviceMethod.NumberOfParameters() == 1?
					AsyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request):
					AsyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request, cancel),
				request,
				cancel).Compile();

			return async (r, c) => await lambda(r, c);
		}

		private static Type GetTaskResultTypeFromMethodReturnType(MethodInfo serviceMethod)
		{
			return serviceMethod.ReturnType.GetGenericArguments()[0];
		}
	}
}
