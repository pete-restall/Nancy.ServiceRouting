using System;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;

namespace Restall.Nancy.ServiceRouting.Tests
{
	public static class TypeAssertionExtensions
	{
		public static AndConstraint<Type> Inherit<T>(this TypeAssertions assertions, string because = "", params object[] reasonArgs)
		{
			var type = assertions.Subject;
			Execute.Assertion
				.ForCondition(type.Inherits<T>())
				.BecauseOf(because, reasonArgs)
				.FailWith("Expected type {0} to inherit from {1}{reason}", type, typeof(T));

			return new AndConstraint<Type>(type);
		}
	}
}
