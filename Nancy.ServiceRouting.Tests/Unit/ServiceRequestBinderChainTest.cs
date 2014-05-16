using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class ServiceRequestBinderChainTest
	{
		[Fact]
		public void Constructor_CalledWithNullEnumerableServiceRequestBinders_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new ServiceRequestBinderChain((IEnumerable<IServiceRequestBinder>) null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceRequestBinders");
		}

		[Fact]
		public void Constructor_CalledWithNullParamsServiceRequestBinders_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new ServiceRequestBinderChain(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceRequestBinders");
		}

		[Fact]
		public void CanCreateBindingDelegateFor_CalledWithNullRequestType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var chain = CreateChainWithDummyDependencies();
			chain.Invoking(x => x.CanCreateBindingDelegateFor(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestType");
		}

		private static ServiceRequestBinderChain CreateChainWithDummyDependencies()
		{
			return new ServiceRequestBinderChain(new IServiceRequestBinder[0]);
		}

		[Theory, NancyAutoData]
		public void CanCreateBindingDelegateFor_CalledWhenNoInnerBinders_ExpectFalseIsReturned(Type requestType)
		{
			CreateChainWithDummyDependencies()
				.CanCreateBindingDelegateFor(requestType).Should().BeFalse();
		}

		[Theory, NancyAutoData]
		public void CanCreateBindingDelegateFor_CalledWhenAllInnerBindersReturnFalse_ExpectFalseIsReturned(
			[WithinInclusiveRange(1, 10)] int numberOfInnerBinders, Type requestType)
		{
			var innerBinders = CreateNumberOfStubServiceRequestBindersUnableToCreateDelegate(numberOfInnerBinders, requestType);
			var chain = new ServiceRequestBinderChain(innerBinders);
			chain.CanCreateBindingDelegateFor(requestType).Should().BeFalse();
		}

		private static IEnumerable<IServiceRequestBinder> CreateNumberOfStubServiceRequestBindersUnableToCreateDelegate(
			int numberOfBinders, Type requestType)
		{
			var innerBinders = numberOfBinders.Select(x => MockRepository.GenerateStub<IServiceRequestBinder>()).ToArray();
			innerBinders.ForEach(inv => inv.Stub(x => x.CanCreateBindingDelegateFor(Arg<Type>.Is.Same(requestType))).Return(false));
			return innerBinders;
		}

		[Theory, NancyAutoData]
		public void CanCreateBindingDelegateFor_CalledWhenAtLeastOneInnerBinderReturnsTrue_ExpectTrueIsReturned(
			[WithinInclusiveRange(0, 10)] int numberOfFalseInnerBinders,
			[WithinInclusiveRange(1, 10)] int numberOfTrueInnerBinders,
			Type requestType)
		{
			var falseBinders = CreateNumberOfStubServiceRequestBindersUnableToCreateDelegate(numberOfFalseInnerBinders, requestType);
			var trueBinders = CreateNumberOfStubServiceRequestBindersAbleToCreateDelegate(numberOfTrueInnerBinders, requestType);

			var chain = new ServiceRequestBinderChain(falseBinders.Concat(trueBinders).Shuffle());
			chain.CanCreateBindingDelegateFor(requestType).Should().BeTrue();
		}

		private static IServiceRequestBinder[] CreateNumberOfStubServiceRequestBindersAbleToCreateDelegate(
			int numberOfBinders, Type requestType)
		{
			var trueBinders = numberOfBinders.Select(x => MockRepository.GenerateStub<IServiceRequestBinder>()).ToArray();
			trueBinders.ForEach(inv => inv.Stub(x => x.CanCreateBindingDelegateFor(Arg<Type>.Is.Same(requestType))).Return(true));
			return trueBinders;
		}

		[Theory, NancyAutoData]
		public void CanCreateBindingDelegateFor_CalledMultipleTimes_ExpectEnumerableOfInnerBindersIsOnlyEnumeratedOnce(
			Type requestType1, Type requestType2)
		{
			var innerBinders = Mock.Enumerable<IServiceRequestBinder>();
			var chain = new ServiceRequestBinderChain(innerBinders);
			chain.CanCreateBindingDelegateFor(requestType1);
			chain.CanCreateBindingDelegateFor(requestType2);

			innerBinders.AssertWasCalled(x => x.GetEnumerator(), x => x.Repeat.Once());
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_CalledWithNullRequestType_ExpectArgumentNullExceptionWithCorrectParamName(
			ServiceRequestBinderContext context)
		{
			CreateChainWithDummyDependencies().Invoking(x => x.CreateBindingDelegate(null, context))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestType");
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_CalledWithNullContext_ExpectArgumentNullExceptionWithCorrectParamName(
			Type requestType)
		{
			CreateChainWithDummyDependencies().Invoking(x => x.CreateBindingDelegate(requestType, null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("context");
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_CalledWhenNoInnerBinders_ExpectArgumentExceptionWithCorrectParamName(
			Type requestType, ServiceRequestBinderContext context)
		{
			new ServiceRequestBinderChain(new IServiceRequestBinder[0])
				.Invoking(x => x.CreateBindingDelegate(requestType, context))
					.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("requestType");
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_CalledWhenNoInnerBindersCanCreateDelegate_ExpectArgumentExceptionWithCorrectParamName(
			[WithinInclusiveRange(1, 10)] int numberOfInnerBinders, Type requestType, ServiceRequestBinderContext context)
		{
			var innerBinders = CreateNumberOfStubServiceRequestBindersUnableToCreateDelegate(numberOfInnerBinders, requestType);
			new ServiceRequestBinderChain(innerBinders)
				.Invoking(x => x.CreateBindingDelegate(requestType, context))
					.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("requestType");
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_CalledWhenAtLeastOneInnerBinderCanCreateDelegate_ExpectReturnedDelegateIsFromInnerBinder(
			[WithinInclusiveRange(0, 10)] int numberOfFalseInnerBinders,
			[WithinInclusiveRange(1, 10)] int numberOfTrueInnerBinders,
			Type requestType,
			ServiceRequestBinderContext context)
		{
			Func<object, object> constructedDelegate = x => new object();
			var falseBinders = CreateNumberOfStubServiceRequestBindersUnableToCreateDelegate(numberOfFalseInnerBinders, requestType);
			var trueBinders = CreateNumberOfStubServiceRequestBindersAbleToCreateDelegate(numberOfTrueInnerBinders, requestType);
			trueBinders.ForEach(inv => inv.Stub(
				x => x.CreateBindingDelegate(Arg<Type>.Is.Same(requestType), Arg<ServiceRequestBinderContext>.Is.Same(context)))
					.Return(constructedDelegate));

			var chain = new ServiceRequestBinderChain(falseBinders.Concat(trueBinders).Shuffle());
			chain.CreateBindingDelegate(requestType, context).Should().BeSameAs(constructedDelegate);
		}

		[Theory, NancyAutoData]
		public void CreateBindingDelegate_CalledMultipleTimes_ExpectEnumerableOfInnerBindersIsOnlyEnumeratedOnce(
			Type requestType1, ServiceRequestBinderContext context1, Type requestType2, ServiceRequestBinderContext context2)
		{
			var innerBinders = Mock.Enumerable<IServiceRequestBinder>();
			var chain = new ServiceRequestBinderChain(innerBinders);
			chain.Invoking(x => x.CreateBindingDelegate(requestType1, context1)).ShouldThrow<Exception>();
			chain.Invoking(x => x.CreateBindingDelegate(requestType2, context2)).ShouldThrow<Exception>();

			innerBinders.AssertWasCalled(x => x.GetEnumerator(), x => x.Repeat.Once());
		}
	}
}
