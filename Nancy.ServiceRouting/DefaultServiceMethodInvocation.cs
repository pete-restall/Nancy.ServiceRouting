using System;
using System.Linq.Expressions;
using System.Reflection;
using NullGuard;

namespace Restall.Nancy.ServiceRouting
{
	public class DefaultServiceMethodInvocation: IServiceMethodInvocation
	{
		public Func<object, object> CreateInvocationDelegate(Func<object> serviceFactory, MethodInfo serviceMethod, [AllowNull] object defaultResponse)
		{
			if (serviceMethod.GetParameters().Length != 1)
				throw new ArgumentException("Method " + serviceMethod + " should have a single parameter", "serviceMethod");

			ParameterExpression request = Expression.Parameter(typeof(object), "request");
			if (serviceMethod.ReturnType == typeof(void))
			{
				return Expression.Lambda<Func<object, object>>(
					Expression.Block(
						CreateCallExpression(serviceFactory, serviceMethod, request),
						Expression.Constant(defaultResponse, typeof(object))),
					request).Compile();
			}

			return Expression.Lambda<Func<object, object>>(
				CreateCallExpression(serviceFactory, serviceMethod, request),
				request).Compile();
		}

		private static MethodCallExpression CreateCallExpression(Func<object> serviceFactory, MethodInfo serviceMethod, Expression request)
		{
			return Expression.Call(
				Expression.Convert(
					Expression.Invoke(Expression.Constant(serviceFactory)),
					serviceMethod.DeclaringType),
				serviceMethod,
				Expression.Convert(request, serviceMethod.TypeOfFirstParameter()));
		}
	}
}
