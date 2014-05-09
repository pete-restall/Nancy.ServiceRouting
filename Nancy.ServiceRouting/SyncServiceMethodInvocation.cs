using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NullGuard;

namespace Restall.Nancy.ServiceRouting
{
	public class SyncServiceMethodInvocation: IServiceMethodInvocation
	{
		public bool CanCreateInvocationDelegateFor(MethodInfo serviceMethod)
		{
			return !serviceMethod.IsAsyncDecorated() && serviceMethod.NumberOfParameters() == 1 && !typeof(Task).IsAssignableFrom(serviceMethod.ReturnType);
		}

		public Delegate CreateInvocationDelegate(
			Func<object> serviceFactory, Func<object, object> requestBinder, MethodInfo serviceMethod, [AllowNull] object defaultResponse)
		{
			if (!this.CanCreateInvocationDelegateFor(serviceMethod))
			{
				throw new ArgumentException(
					"Method " + serviceMethod + " should have a single parameter, not return a Task and not be marked async",
					"serviceMethod");
			}

			ParameterExpression request = Expression.Parameter(typeof(object), "request");
			if (serviceMethod.ReturnType == typeof(void))
			{
				return Expression.Lambda<Func<object, object>>(
					Expression.Block(
						CreateCallExpression(serviceFactory, requestBinder, serviceMethod, request),
						Expression.Constant(defaultResponse, typeof(object))),
					request).Compile();
			}

			return Expression.Lambda<Func<object, object>>(
				CreateCallExpression(serviceFactory, requestBinder, serviceMethod, request),
				request).Compile();
		}

		private static MethodCallExpression CreateCallExpression(
			Func<object> serviceFactory, Func<object, object> requestBinder, MethodInfo serviceMethod, Expression request)
		{
			return Expression.Call(
				Expression.Convert(
					Expression.Invoke(Expression.Constant(serviceFactory)),
					serviceMethod.DeclaringType),
				serviceMethod,
				Expression.Convert(
					Expression.Invoke(Expression.Constant(requestBinder), request),
					serviceMethod.TypeOfFirstParameter()));
		}
	}
}
