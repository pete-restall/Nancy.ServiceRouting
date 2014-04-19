using System;
using System.Reflection;
using System.Threading;
using NullGuard;

namespace Nancy.ServiceRouting
{
	public class RouteDispatchBuilder
	{
		private class DispatchContext
		{
			public Func<object, object> CreateDispatch()
			{
				var invoke = this.CreateInvoke();
				var bindModel = this.CreateRequestBinder();
				return request => invoke(bindModel(request));
			}

			private Func<object, object> CreateInvoke()
			{
				return this.ServiceMethodInvocation.CreateInvocationDelegate(() => this.ServiceFactory(this.ServiceType), this.Method, this.DefaultResponse);
			}

			private Func<object, object> CreateRequestBinder()
			{
				return this.RequestMessageBinder.CreateBindingDelegate(this.Module, this.Method.TypeOfFirstParameter());
			}

			public Func<Type, object> ServiceFactory;
			public IServiceRequestBinder RequestMessageBinder;
			public IServiceMethodInvocation ServiceMethodInvocation;
			public Type ServiceType;
			public MethodInfo Method;
			public object DefaultResponse = HttpStatusCode.NoContent;
			public NancyModule Module;
		}

		private DispatchContext dispatchContext = new DispatchContext();

		public RouteDispatchBuilder WithServiceFactory(Func<Type, object> serviceFactory)
		{
			this.dispatchContext.ServiceFactory = serviceFactory;
			return this;
		}

		public RouteDispatchBuilder WithServiceMethodInvocation(IServiceMethodInvocation serviceMethodInvocation)
		{
			this.dispatchContext.ServiceMethodInvocation = serviceMethodInvocation;
			return this;
		}

		public RouteDispatchBuilder WithRequestMessageBinder(IServiceRequestBinder requestMessageBinder)
		{
			this.dispatchContext.RequestMessageBinder = requestMessageBinder;
			return this;
		}

		public RouteDispatchBuilder WithModule(NancyModule module)
		{
			this.dispatchContext.Module = module;
			return this;
		}

		public RouteDispatchBuilder WithServiceType(Type serviceType)
		{
			this.dispatchContext.ServiceType = serviceType;
			return this;
		}

		public RouteDispatchBuilder WithMethod(MethodInfo method)
		{
			this.dispatchContext.Method = method;
			return this;
		}

		public RouteDispatchBuilder WithDefaultResponse([AllowNull] object defaultResponse)
		{
			this.dispatchContext.DefaultResponse = defaultResponse;
			return this;
		}

		public Func<object, object> Build()
		{
			var savedBindingContext = Interlocked.Exchange(ref this.dispatchContext, new DispatchContext());
			return savedBindingContext.CreateDispatch();
		}
	}
}
