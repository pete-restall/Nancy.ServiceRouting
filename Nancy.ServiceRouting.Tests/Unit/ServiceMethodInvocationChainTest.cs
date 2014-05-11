using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class ServiceMethodInvocationChainTest
	{
		[Fact]
		public void Constructor_CalledWithNullEnumerableServiceMethodInvocations_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new ServiceMethodInvocationChain((IEnumerable<IServiceMethodInvocation>) null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethodInvocations");
		}

		[Fact]
		public void Constructor_CalledWithNullParamsServiceMethodInvocations_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new ServiceMethodInvocationChain(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethodInvocations");
		}

		[Fact]
		public void CanCreateInvocationDelegateFor_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var chain = CreateChainWithDummyDependencies();
			chain.Invoking(x => x.CanCreateInvocationDelegateFor(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		private static ServiceMethodInvocationChain CreateChainWithDummyDependencies()
		{
			return new ServiceMethodInvocationChain(new IServiceMethodInvocation[0]);
		}

		[Theory, NancyAutoData]
		public void CanCreateInvocationDelegateFor_CalledWhenNoInnerInvocations_ExpectFalseIsReturned(MethodInfo method)
		{
			CreateChainWithDummyDependencies()
				.CanCreateInvocationDelegateFor(method).Should().BeFalse();
		}

		[Theory, NancyAutoData]
		public void CanCreateInvocationDelegateFor_CalledWhenAllInnerInvocationsReturnFalse_ExpectFalseIsReturned(
			[WithinInclusiveRange(1, 10)] int numberOfInnerInvocations, MethodInfo method)
		{
			var innerInvocations = CreateNumberOfStubServiceMethodInvocationsUnableToCreateDelegate(numberOfInnerInvocations, method);
			var chain = new ServiceMethodInvocationChain(innerInvocations);
			chain.CanCreateInvocationDelegateFor(method).Should().BeFalse();
		}

		private static IEnumerable<IServiceMethodInvocation> CreateNumberOfStubServiceMethodInvocationsUnableToCreateDelegate(
			int numberOfInvocations, MethodInfo method)
		{
			var innerInvocations = numberOfInvocations.Select(x => MockRepository.GenerateStub<IServiceMethodInvocation>()).ToArray();
			innerInvocations.ForEach(inv => inv.Stub(x => x.CanCreateInvocationDelegateFor(Arg<MethodInfo>.Is.Same(method))).Return(false));
			return innerInvocations;
		}

		[Theory, NancyAutoData]
		public void CanCreateInvocationDelegateFor_CalledWhenAtLeastOneInnerInvocationReturnsTrue_ExpectTrueIsReturned(
			[WithinInclusiveRange(0, 10)] int numberOfFalseInnerInvocations,
			[WithinInclusiveRange(1, 10)] int numberOfTrueInnerInvocations,
			MethodInfo method)
		{
			var falseInvocations = CreateNumberOfStubServiceMethodInvocationsUnableToCreateDelegate(numberOfFalseInnerInvocations, method);
			var trueInvocations = CreateNumberOfStubServiceMethodInvocationsAbleToCreateDelegate(numberOfTrueInnerInvocations, method);

			var chain = new ServiceMethodInvocationChain(falseInvocations.Concat(trueInvocations).Shuffle());
			chain.CanCreateInvocationDelegateFor(method).Should().BeTrue();
		}

		private static IServiceMethodInvocation[] CreateNumberOfStubServiceMethodInvocationsAbleToCreateDelegate(
			int numberOfInvocations, MethodInfo method)
		{
			var trueInvocations = numberOfInvocations.Select(x => MockRepository.GenerateStub<IServiceMethodInvocation>()).ToArray();
			trueInvocations.ForEach(inv => inv.Stub(x => x.CanCreateInvocationDelegateFor(Arg<MethodInfo>.Is.Same(method))).Return(true));
			return trueInvocations;
		}

		[Theory, NancyAutoData]
		public void CanCreateInvocationDelegateFor_CalledMultipleTimes_ExpectEnumerableOfInnerInvocationsIsOnlyEnumeratedOnce(
			MethodInfo method1, MethodInfo method2)
		{
			var innerInvocations = Mock.Enumerable<IServiceMethodInvocation>();
			var chain = new ServiceMethodInvocationChain(innerInvocations);
			chain.CanCreateInvocationDelegateFor(method1);
			chain.CanCreateInvocationDelegateFor(method2);

			innerInvocations.AssertWasCalled(x => x.GetEnumerator(), x => x.Repeat.Once());
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullServiceMethod_ExpectArgumentNullExceptionWithCorrectParamName(
			ServiceMethodInvocationContext context)
		{
			CreateChainWithDummyDependencies().Invoking(x => x.CreateInvocationDelegate(null, context))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWithNullContext_ExpectArgumentNullExceptionWithCorrectParamName(
			MethodInfo method)
		{
			CreateChainWithDummyDependencies().Invoking(x => x.CreateInvocationDelegate(method, null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("context");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWhenNoInnerInvocations_ExpectArgumentExceptionWithCorrectParamName(
			MethodInfo method, ServiceMethodInvocationContext context)
		{
			new ServiceMethodInvocationChain(new IServiceMethodInvocation[0])
				.Invoking(x => x.CreateInvocationDelegate(method, context))
					.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWhenNoInnerInvocationsCanCreateDelegate_ExpectArgumentExceptionWithCorrectParamName(
			[WithinInclusiveRange(1, 10)] int numberOfInnerInvocations, MethodInfo method, ServiceMethodInvocationContext context)
		{
			var innerInvocations = CreateNumberOfStubServiceMethodInvocationsUnableToCreateDelegate(numberOfInnerInvocations, method);
			new ServiceMethodInvocationChain(innerInvocations)
				.Invoking(x => x.CreateInvocationDelegate(method, context))
					.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("serviceMethod");
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledWhenAtLeastOneInnerInvocationCanCreateDelegate_ExpectReturnedDelegateIsFromInnerInvocation(
			[WithinInclusiveRange(0, 10)] int numberOfFalseInnerInvocations,
			[WithinInclusiveRange(1, 10)] int numberOfTrueInnerInvocations,
			MethodInfo method,
			ServiceMethodInvocationContext context)
		{
			Action constructedDelegate = () => { };
			var falseInvocations = CreateNumberOfStubServiceMethodInvocationsUnableToCreateDelegate(numberOfFalseInnerInvocations, method);
			var trueInvocations = CreateNumberOfStubServiceMethodInvocationsAbleToCreateDelegate(numberOfTrueInnerInvocations, method);
			trueInvocations.ForEach(inv => inv.Stub(
				x => x.CreateInvocationDelegate(Arg<MethodInfo>.Is.Same(method), Arg<ServiceMethodInvocationContext>.Is.Same(context)))
					.Return(constructedDelegate));

			var chain = new ServiceMethodInvocationChain(falseInvocations.Concat(trueInvocations).Shuffle());
			chain.CreateInvocationDelegate(method, context).Should().BeSameAs(constructedDelegate);
		}

		[Theory, NancyAutoData]
		public void CreateInvocationDelegate_CalledMultipleTimes_ExpectEnumerableOfInnerInvocationsIsOnlyEnumeratedOnce(
			MethodInfo method1, ServiceMethodInvocationContext context1, MethodInfo method2, ServiceMethodInvocationContext context2)
		{
			var innerInvocations = Mock.Enumerable<IServiceMethodInvocation>();
			var chain = new ServiceMethodInvocationChain(innerInvocations);
			chain.Invoking(x => x.CreateInvocationDelegate(method1, context1)).ShouldThrow<Exception>();
			chain.Invoking(x => x.CreateInvocationDelegate(method2, context2)).ShouldThrow<Exception>();

			innerInvocations.AssertWasCalled(x => x.GetEnumerator(), x => x.Repeat.Once());
		}
	}
}
