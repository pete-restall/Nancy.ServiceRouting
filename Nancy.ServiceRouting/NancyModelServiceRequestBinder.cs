using System;
using System.Linq.Expressions;
using System.Reflection;
using Nancy;
using Nancy.ModelBinding;

namespace Restall.Nancy.ServiceRouting
{
	public class NancyModelServiceRequestBinder: IServiceRequestBinder
	{
		private readonly MethodInfo nancyModelBinder;

		public NancyModelServiceRequestBinder(): this(m => m.Bind<object>())
		{
		}

		public NancyModelServiceRequestBinder(Expression<Action<NancyModule>> nancyModelBindingExpression)
		{
			this.nancyModelBinder = InfoOf.Method(nancyModelBindingExpression).GetGenericMethodDefinition();
		}

		public Func<object, object> CreateBindingDelegate(NancyModule module, Type requestType)
		{
			MethodInfo binder = this.nancyModelBinder.MakeGenericMethod(requestType);
			ParameterExpression requestParameters = Expression.Parameter(typeof(object), "requestParameters");
			return Expression.Lambda<Func<object, object>>(
				Expression.Call(
					binder,
					Expression.Constant(module, typeof(NancyModule))),
				requestParameters).Compile();
		}
	}
}
