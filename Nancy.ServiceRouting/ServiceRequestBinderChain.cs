using System;
using System.Collections.Generic;
using System.Linq;

namespace Restall.Nancy.ServiceRouting
{
	public class ServiceRequestBinderChain: IServiceRequestBinder
	{
		private readonly IServiceRequestBinder[] serviceRequestBinders;

		public ServiceRequestBinderChain(IEnumerable<IServiceRequestBinder> serviceRequestBinders):
			this(serviceRequestBinders.ToArray())
		{
		}

		public ServiceRequestBinderChain(params IServiceRequestBinder[] serviceRequestBinders)
		{
			this.serviceRequestBinders = serviceRequestBinders;
		}

		public bool CanCreateBindingDelegateFor(Type requestType)
		{
			return this.serviceRequestBinders.Any(x => x.CanCreateBindingDelegateFor(requestType));
		}

		public Func<object, object> CreateBindingDelegate(Type requestType, ServiceRequestBinderContext context)
		{
			IServiceRequestBinder binder = this.serviceRequestBinders.FirstOrDefault(x => x.CanCreateBindingDelegateFor(requestType));
			if (binder == null)
			{
				throw new ArgumentException(
					"Type " + requestType + " cannot be bound from a request to a DTO as its type is unknown",
					nameof(requestType));
			}

			return binder.CreateBindingDelegate(requestType, context);
		}
	}
}
