using System;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class NancyModelServiceRequestBinderTest
	{
		private class Request { }

		private class SomethingOtherThanRequest { }

		private abstract class AbstractType { }

		private interface IInterfaceType { }

		private class GenericType<T> { }

		private struct StructType { }

		private enum EnumType { }

		[Fact]
		public void Constructor_CalledWithNullNancyModelBindingExpression_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new NancyModelServiceRequestBinder(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("nancyModelBindingExpression");
		}

		[Fact]
		public void CanCreateBindingDelegateFor_CalledWithNullRequestType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new NancyModelServiceRequestBinder().Invoking(x => x.CanCreateBindingDelegateFor(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestType");
		}

		[Theory]
		[NancyInlineAutoData(typeof(AbstractType))]
		[NancyInlineAutoData(typeof(IInterfaceType))]
		[NancyInlineAutoData(typeof(GenericType<>))]
		public void CanCreateBindingDelegateFor_CalledWithNonConcreteClassType_ExpectFalseIsReturned(Type nonConcreteType)
		{
			new NancyModelServiceRequestBinder().CanCreateBindingDelegateFor(nonConcreteType)
				.Should().BeFalse();
		}

		[Theory]
		[NancyInlineAutoData(typeof(Request))]
		[NancyInlineAutoData(typeof(StructType))]
		[NancyInlineAutoData(typeof(EnumType))]
		public void CanCreateBindingDelegateFor_CalledWithConcreteClassType_ExpectTrueIsReturned(Type concreteType)
		{
			new NancyModelServiceRequestBinder().CanCreateBindingDelegateFor(concreteType)
				.Should().BeTrue();
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_CalledWithNullRequestType_ExpectArgumentNullExceptionWithCorrectParamName(
			ServiceRequestBinderContext context)
		{
			new NancyModelServiceRequestBinder().Invoking(x => x.CreateBindingDelegate(null, context))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestType");
		}

		[Theory]
		[NancyInlineAutoData(typeof(AbstractType))]
		[NancyInlineAutoData(typeof(IInterfaceType))]
		[NancyInlineAutoData(typeof(GenericType<>))]
		public void CreateBindingDelegate_CalledWithNonConcreteType_ExpectArgumentExceptionWithCorrectParamName(
			Type nonConcreteType, ServiceRequestBinderContext context)
		{
			new NancyModelServiceRequestBinder().Invoking(x => x.CreateBindingDelegate(nonConcreteType, context))
				.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("requestType");
		}

		[Theory]
		[NancyInlineAutoData(typeof(Request))]
		[NancyInlineAutoData(typeof(StructType))]
		[NancyInlineAutoData(typeof(EnumType))]
		public void CreateBindingDelegate_CalledWithConcreteClassType_ExpectDelegateIsReturned(
			Type concreteType, ServiceRequestBinderContext context)
		{
			new NancyModelServiceRequestBinder().Invoking(x => x.CreateBindingDelegate(concreteType, context))
				.Should().NotBeNull();
		}

		[Fact]
		public void CreateBindingDelegate_CalledWithNullContext_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new NancyModelServiceRequestBinder().Invoking(x => x.CreateBindingDelegate(typeof(Request), null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("context");
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_Called_ExpectReturnedDelegateCallsGenericVersionOfDelegatePassedToConstructor(
			ServiceRequestBinderContext context, object requestParameters)
		{
			var binder = new NancyModelServiceRequestBinder(m => m.StubNancyModelBind<SomethingOtherThanRequest>());
			var lambda = binder.CreateBindingDelegate(typeof(Request), context);
			StubNancyModelBinding.StubBoundModel = new Request();
			StubNancyModelBinding.SpiedModule = null;

			lambda(requestParameters).Should().BeSameAs(StubNancyModelBinding.StubBoundModel);
			StubNancyModelBinding.SpiedModule.Should().BeSameAs(context.NancyModule);
		}
	}
}
