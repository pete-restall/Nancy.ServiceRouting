using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class NamedRouteAttributeTest
	{
		[Fact]
		public void ExpectNamedRouteAttributeInheritsFromRouteAttribute()
		{
			typeof(NamedRouteAttribute).Should().Inherit<RouteAttribute>();
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullName_ExpectArgumentNullExceptionWithCorrectParamName(string path)
		{
			Action constructor = () => new NamedRouteAttribute(null, path);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("name");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithEmptyName_ExpectArgumentExceptionWithCorrectParamName(string path)
		{
			Action constructor = () => new NamedRouteAttribute(string.Empty, path);
			constructor.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("name");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithWhitespaceName_ExpectArgumentExceptionWithCorrectParamName([Whitespace] string whitespace, string path)
		{
			Action constructor = () => new NamedRouteAttribute(whitespace, path);
			constructor.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("name");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullPath_ExpectArgumentNullExceptionWithCorrectParamName(string name)
		{
			Action constructor = () => new NamedRouteAttribute(name, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("path");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullVerbs_ExpectArgumentNullExceptionWithCorrectParamName(string name, string path)
		{
			Action constructor = () => new NamedRouteAttribute(name, path, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("verbs");
		}

		[Theory, NancyAutoData]
		public void WireToMethod_CalledWithNullMethod_ExpectArgumentNullExceptionWithCorrectParamName(NamedRouteAttribute attribute)
		{
			attribute.Invoking(x => x.WireToMethod(null))
				.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("method");
		}

		[Theory, NancyAutoData]
		public void WireToMethod_Called_ExpectRoutesAreReturnedWithSameNameAndPathAndMethodForEachVerb(
			string name, string path, string[] verbs, MethodInfo method)
		{
			var attribute = new NamedRouteAttribute(name, path, verbs.TakeAtLeastOneItem());
			attribute.WireToMethod(method).ShouldBeEquivalentTo(attribute.Verbs.Select(v => new Route(attribute.Name, v, attribute.Path, method)));
		}

		[Theory, NancyAutoData]
		public void Name_Get_ExpectTrimmedValueOfThatPassedToConstructor(string name, string path, [Whitespace] string whitespace)
		{
			new NamedRouteAttribute(whitespace + name + whitespace, path).Name.Should().Be(name);
		}

		[Theory, NancyAutoData]
		public void Path_Get_ExpectSameValueAsPassedToConstructor(string name, string path)
		{
			new NamedRouteAttribute(name, path).Path.Should().Be(path);
		}

		[Theory, NancyAutoData]
		public void Verbs_GetAfterConstructorWithNoVerbs_ExpectOnlyGetIsReturned(string name, string path)
		{
			new NamedRouteAttribute(name, path).Verbs.Should().Equal(new[] {"GET"});
		}

		[Theory, NancyAutoData]
		public void Verbs_Get_ExpectConstructorPassedSameVerbsToBaseClassConstructor(string name, string path, string[] verbs)
		{
			new NamedRouteAttribute(name, path, verbs).Verbs.Should().Equal(verbs.Select(v => v.Trim().ToUpper()));
		}
	}
}
