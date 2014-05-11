using System.Linq.Expressions;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	internal class AsyncServiceMethodCall
	{
		public static MethodCallExpression CreateCallExpression(
			MethodInfo serviceMethod, ServiceMethodInvocationContext context, ParameterExpression request)
		{
			return SyncServiceMethodCall.CreateCallExpression(serviceMethod, context, request);
		}

		public static MethodCallExpression CreateCallExpression(
			MethodInfo serviceMethod, ServiceMethodInvocationContext context, ParameterExpression request, ParameterExpression cancel)
		{
			return Expression.Call(
				Expression.Convert(
					Expression.Invoke(Expression.Constant(context.ServiceFactory)),
					serviceMethod.DeclaringType),
				serviceMethod,
				Expression.Convert(
					Expression.Invoke(
						Expression.Constant(context.RequestBinder),
						request),
					serviceMethod.TypeOfFirstParameter()),
				cancel);
		}
	}
}
