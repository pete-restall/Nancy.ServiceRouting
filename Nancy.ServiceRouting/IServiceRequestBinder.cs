using System;

namespace Restall.Nancy.ServiceRouting
{
	public interface IServiceRequestBinder
	{
		bool CanCreateBindingDelegateFor(Type requestType);
		Func<object, object> CreateBindingDelegate(Type requestType, ServiceRequestBinderContext context);
	}
}
