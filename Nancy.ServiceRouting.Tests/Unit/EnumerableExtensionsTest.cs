using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Nancy.ServiceRouting.Tests.Unit
{
	public class EnumerableExtensionsTest
	{
		[Fact]
		public void ForEach_CalledWithNullSource_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			IEnumerable<object> nullEnumerable = null;
			nullEnumerable.Invoking(x => x.ForEach(y => { }))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("source");
		}

		[Fact]
		public void ForEach_CalledWithNullAction_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			new object[0].Invoking(x => x.ForEach(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("action");
		}

		[Fact]
		public void ForEach_CalledWithAnyNumberOfItems_ExpectActionIsAppliedToEachItemInOrder()
		{
			var items = new object[new Random().Next(0, 10)];
			for (int i = 0; i < items.Length; i++)
				items[i] = new object();

			var processedItems = new List<object>();
			items.ForEach(processedItems.Add);
			processedItems.Should().ContainInOrder(items).And.HaveSameCount(items);
		}
	}
}
