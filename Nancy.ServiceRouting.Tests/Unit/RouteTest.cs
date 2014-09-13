using System;
using System.Reflection;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class RouteTest
	{
		[Theory, NancyAutoData]
		public void Constructor_NameOverloadCalledWithNullName_ExpectArgumentNullExceptionWithCorrectParamName(string verb, string path, MethodInfo method)
		{
			Action constructor = () => new Route(null, verb, path, method);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("name");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullVerb_ExpectArgumentNullExceptionWithCorrectParamName(string path, MethodInfo method)
		{
			Action constructor = () => new Route(null, path, method);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("verb");
		}

		[Theory, NancyAutoData]
		public void Constructor_NameOverloadCalledWithNullVerb_ExpectArgumentNullExceptionWithCorrectParamName(string name, string path, MethodInfo method)
		{
			Action constructor = () => new Route(name, null, path, method);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("verb");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullPath_ExpectArgumentNullExceptionWithCorrectParamName(string verb, MethodInfo method)
		{
			Action constructor = () => new Route(verb, null, method);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("path");
		}

		[Theory, NancyAutoData]
		public void Constructor_NameOverloadCalledCalledWithNullPath_ExpectArgumentNullExceptionWithCorrectParamName(
			string name, string verb, MethodInfo method)
		{
			Action constructor = () => new Route(name, verb, null, method);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("path");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullMethod_ExpectArgumentNullExceptionWithCorrectParamName(string verb, string path)
		{
			Action constructor = () => new Route(verb, path, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("method");
		}

		[Theory, NancyAutoData]
		public void Constructor_NameOverloadCalledCalledWithNullMethod_ExpectArgumentNullExceptionWithCorrectParamName(
			string name, string verb, string path)
		{
			Action constructor = () => new Route(name, verb, path, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("method");
		}

		[Theory, NancyAutoData]
		public void Name_GetWhenNoNamePassedToConstructor_ExpectEmptyStringIsReturned(string verb, string path, MethodInfo method)
		{
			new Route(verb, path, method).Name.Should().BeEmpty();
		}

		[Theory, NancyAutoData]
		public void Name_Get_ExpectSameValueAsPassedToConstructor(string name, string verb, string path, MethodInfo method)
		{
			new Route(name, verb, path, method).Name.Should().Be(name);
		}

		[Theory, NancyAutoData]
		public void Verb_Get_ExpectSameValueAsPassedToConstructor(string name, string verb, string path, MethodInfo method)
		{
			new Route(verb, path, method).Verb.Should().Be(verb);
			new Route(name, verb, path, method).Verb.Should().Be(verb);
		}

		[Theory, NancyAutoData]
		public void Path_Get_ExpectSameValueAsPassedToConstructor(string name, string verb, string path, MethodInfo method)
		{
			new Route(verb, path, method).Path.Should().Be(path);
			new Route(name, verb, path, method).Path.Should().Be(path);
		}

		[Theory, NancyAutoData]
		public void Method_Get_ExpectSameValueAsPassedToConstructor(string name, string verb, string path, MethodInfo method)
		{
			((object) new Route(verb, path, method).Method).Should().BeSameAs(method);
			((object) new Route(name, verb, path, method).Method).Should().BeSameAs(method);
		}
	}
}
