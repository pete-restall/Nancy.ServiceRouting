using System;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class ServiceMethodInvocationContextTest
	{
		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullServiceFactory_ExpectArgumentNullExceptionWithCorrectParamName(object obj)
		{
			Action constructor = () => new ServiceMethodInvocationContext(null, x => obj, obj);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceFactory");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullRequestBinder_ExpectArgumentNullExceptionWithCorrectParamName(object obj)
		{
			Action constructor = () => new ServiceMethodInvocationContext(() => obj, null, obj);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestBinder");
		}

		[Theory, NancyAutoData]
		public void ServiceFactory_Get_ExpectSameValueAsPassedToConstructor(object obj)
		{
			Func<object> serviceFactory = () => obj;
			new ServiceMethodInvocationContext(serviceFactory, x => obj, obj)
				.ServiceFactory.Should().BeSameAs(serviceFactory);
		}

		[Theory, NancyAutoData]
		public void RequestBinder_Get_ExpectSameValueAsPassedToConstructor(object obj)
		{
			Func<object, object> requestBinder = x => obj;
			new ServiceMethodInvocationContext(() => obj, requestBinder, obj)
				.RequestBinder.Should().BeSameAs(requestBinder);
		}

		[Theory, NancyAutoData]
		public void DefaultResponse_Get_ExpectSameValueAsPassedToConstructor(object obj, object defaultResponse)
		{
			new ServiceMethodInvocationContext(() => obj, x => obj, defaultResponse)
				.DefaultResponse.Should().BeSameAs(defaultResponse);
		}

		[Theory, NancyAutoData]
		public void DefaultResponse_GetWhenNullPassedToConstructor_ExpectNullIsReturned(object obj)
		{
			new ServiceMethodInvocationContext(() => obj, x => obj, null)
				.DefaultResponse.Should().BeNull();
		}
	}
}
