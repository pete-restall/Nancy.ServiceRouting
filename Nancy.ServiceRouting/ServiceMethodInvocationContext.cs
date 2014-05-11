using System;
using NullGuard;

namespace Restall.Nancy.ServiceRouting
{
	public class ServiceMethodInvocationContext
	{
		public ServiceMethodInvocationContext(Func<object> serviceFactory, Func<object, object> requestBinder, [AllowNull] object defaultResponse)
		{
			this.ServiceFactory = serviceFactory;
			this.RequestBinder = requestBinder;
			this.DefaultResponse = defaultResponse;
		}

		public Func<object> ServiceFactory { get; private set; }

		public Func<object, object> RequestBinder { get; private set; }

		[AllowNull]
		public object DefaultResponse { get; private set; }
	}
}
