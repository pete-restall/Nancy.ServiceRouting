using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Nancy.ServiceRouting
{
	public static class InfoOf
	{
		public static MethodInfo Method<T>(Expression<Action<T>> methodExpression)
		{
			return ((MethodCallExpression) methodExpression.Body).Method;
		}
	}
}
