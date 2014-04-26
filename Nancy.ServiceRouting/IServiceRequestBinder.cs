using System;
using Nancy;

namespace Restall.Nancy.ServiceRouting
{
	public interface IServiceRequestBinder
	{
		Func<object, object> CreateBindingDelegate(NancyModule module, Type requestType);
	}
}
