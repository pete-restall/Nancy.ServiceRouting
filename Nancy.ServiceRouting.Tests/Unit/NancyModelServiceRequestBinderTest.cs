using System;
using FluentAssertions;
using Nancy;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class NancyModelServiceRequestBinderTest
	{
		private class Request { }

		private class SomethingOtherThanRequest { }

		[Fact]
		public void Constructor_CalledWithNullNancyModelBindingExpression_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new NancyModelServiceRequestBinder(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("nancyModelBindingExpression");
		}

		[Fact]
		public void CreateBindingDelegate_CalledWithNullModule_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new NancyModelServiceRequestBinder().Invoking(x => x.CreateBindingDelegate(null, typeof(Request)))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("module");
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_CalledWithNullRequestType_ExpectArgumentNullExceptionWithCorrectParamName(NancyModule module)
		{
			new NancyModelServiceRequestBinder().Invoking(x => x.CreateBindingDelegate(module, null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestType");
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_Called_ExpectReturnedDelegateCallsGenericVersionOfDelegatePassedToConstructor(
			NancyModule module, object requestParameters)
		{
			var binder = new NancyModelServiceRequestBinder(m => m.StubNancyModelBind<SomethingOtherThanRequest>());
			var lambda = binder.CreateBindingDelegate(module, typeof(Request));
			StubNancyModelBinding.StubBoundModel = new Request();
			StubNancyModelBinding.SpiedModule = null;

			lambda(requestParameters).Should().BeSameAs(StubNancyModelBinding.StubBoundModel);
			StubNancyModelBinding.SpiedModule.Should().BeSameAs(module);
		}
	}
}
