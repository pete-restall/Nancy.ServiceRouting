using System;
using FluentAssertions;
using Xunit;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class InfoOfTest
	{
		private abstract class StubBaseClass
		{
			public abstract void AbstractMethod();
			public virtual void NonOverriddenMethod() { }
			public virtual void OverriddenMethod() { }
		}

		private class StubClass: StubBaseClass
		{
			public void MethodWithoutArguments() { }
			public void MethodWithArguments(object something) { }
			public static void StaticMethod() { }

			public override void AbstractMethod() { }
			public override void OverriddenMethod() { }
		}

		[Fact]
		public void Method_CalledWithNullMethodExpression_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			this.Invoking(x => InfoOf.Method<object>(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("methodExpression");
		}

		[Fact]
		public void Method_CalledWithOverloadedInstanceMember_ExpectMethodInfoOfExpressionIsReturned()
		{
			((object) InfoOf.Method<object>(x => x.ToString()))
				.Should().Be(typeof(object).GetMethod("ToString", new Type[0]));
		}

		[Fact]
		public void Method_CalledWithInstanceMemberOfClass_ExpectMethodInfoOfExpressionIsReturned()
		{
			((object) InfoOf.Method<StubClass>(x => x.MethodWithoutArguments()))
				.Should().Be(typeof(StubClass).GetMethod("MethodWithoutArguments", new Type[0]));
		}

		[Fact]
		public void Method_CalledWithInstanceMemberTakingArguments_ExpectMethodInfoOfExpressionIsReturned()
		{
			((object) InfoOf.Method<StubClass>(x => x.MethodWithArguments(null)))
				.Should().Be(typeof(StubClass).GetMethod("MethodWithArguments", new[] {typeof(object)}));
		}

		[Fact]
		public void Method_CalledWithStaticMember_ExpectMethodInfoOfExpressionIsReturned()
		{
			((object) InfoOf.Method<object>(x => StubClass.StaticMethod()))
				.Should().Be(typeof(StubClass).GetMethod("StaticMethod", new Type[0]));
		}

		[Fact]
		public void Method_CalledWithAbstractMemberOfClass_ExpectLeastDerivedMethodInfoOfExpressionIsReturned()
		{
			((object) InfoOf.Method<StubClass>(x => x.AbstractMethod()))
				.Should().Be(typeof(StubBaseClass).GetMethod("AbstractMethod", new Type[0]));
		}

		[Fact]
		public void Method_CalledWithOverriddenVirtualMemberOfClass_ExpectLeastDerivedMethodInfoOfExpressionIsReturned()
		{
			((object)InfoOf.Method<StubClass>(x => x.OverriddenMethod()))
				.Should().Be(typeof(StubBaseClass).GetMethod("OverriddenMethod", new Type[0]));
		}

		[Fact]
		public void Method_CalledWithNonOverriddenVirtualMemberOfClass_ExpectLeastDerivedMethodInfoOfExpressionIsReturned()
		{
			((object)InfoOf.Method<StubClass>(x => x.NonOverriddenMethod()))
				.Should().Be(typeof(StubBaseClass).GetMethod("NonOverriddenMethod", new Type[0]));
		}
	}
}
