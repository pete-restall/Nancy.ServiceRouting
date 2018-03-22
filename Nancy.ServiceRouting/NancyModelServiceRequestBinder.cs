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

		public bool CanCreateBindingDelegateFor(Type requestType)
		{
			return requestType.IsConcrete();
		}

		public Func<object, object> CreateBindingDelegate(Type requestType, ServiceRequestBinderContext context)
		{
			if (!this.CanCreateBindingDelegateFor(requestType))
				throw new ArgumentException("Type " + requestType + " must be a concrete class", nameof(requestType));

			MethodInfo binder = this.nancyModelBinder.MakeGenericMethod(requestType);
			ParameterExpression requestParameters = Expression.Parameter(typeof(object), "requestParameters");
			return Expression.Lambda<Func<object, object>>(
				Expression.Call(
					binder,
					Expression.Constant(context.NancyModule, typeof(NancyModule))),
				requestParameters).Compile();
		}
	}
}
