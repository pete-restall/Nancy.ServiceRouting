using System;
using System.Linq;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class RouteAttributeTest
	{
		private static readonly string[] Verbs = new[] {"GET", "PUT", "POST", "DELETE", "PATCH"};

		[Fact]
		public void Constructor_CalledWithNullPath_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new RouteAttribute(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("path");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullVerbs_ExpectArgumentNullExceptionWithCorrectParamName(string path)
		{
			Action constructor = () => new RouteAttribute(path, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("verbs");
		}

		[Theory, NancyAutoData]
		public void Path_Get_ExpectSameValueAsPassedToConstructor(string path)
		{
			new RouteAttribute(path).Path.Should().Be(path);
		}

		[Theory, NancyAutoData]
		public void Verbs_GetAfterConstructorWithNoVerbs_ExpectOnlyGetIsReturned(string path)
		{
			new RouteAttribute(path).Verbs.Should().Equal(new[] {"GET"});
		}

		[Theory, NancyAutoData]
		public void Verbs_Get_ExpectUppercasedValuesOfThosePassedToConstructor(string path)
		{
			var verbs = Verbs.TakeAtLeastOneItem();
			new RouteAttribute(path, verbs.Select(x => x.ToLower()).ToArray())
				.Verbs.Should().BeEquivalentTo(verbs.Select(x => x.ToUpper()));
		}

		[Theory, NancyAutoData]
		public void Verbs_Get_ExpectTrimmedValuesOfThosePassedToConstructor(string path, [Whitespace] string whitespace)
		{
			var verbs = Verbs.TakeAtLeastOneItem();
			new RouteAttribute(path, verbs.Select(x => whitespace + x + whitespace).ToArray())
				.Verbs.Should().BeEquivalentTo(verbs.Select(x => x.ToUpper()));
		}
	}
}
