using System;
using System.Reflection;
using FluentAssertions;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class RouteTest
	{
		private class StubService
		{
			public object ServiceMethod(object request) { return new object(); }
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullVerb_ExpectArgumentNullExceptionWithCorrectParamName(string path)
		{
			Action constructor = () => new Route(null, path, DummyMethodInfo);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("verb");
		}

		private static MethodInfo DummyMethodInfo
		{
			get { return InfoOf.Method<StubService>(x => x.ServiceMethod(null)); }
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullPath_ExpectArgumentNullExceptionWithCorrectParamName(string verb)
		{
			Action constructor = () => new Route(verb, null, DummyMethodInfo);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("path");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullMethod_ExpectArgumentNullExceptionWithCorrectParamName(string verb, string path)
		{
			Action constructor = () => new Route(verb, path, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("method");
		}

		[Theory, NancyAutoData]
		public void Verb_Get_ExpectSameValueAsPassedToConstructor(string verb, string path)
		{
			new Route(verb, path, DummyMethodInfo).Verb.Should().Be(verb);
		}

		[Theory, NancyAutoData]
		public void Path_Get_ExpectSameValueAsPassedToConstructor(string verb, string path)
		{
			new Route(verb, path, DummyMethodInfo).Path.Should().Be(path);
		}

		[Theory, NancyAutoData]
		public void Method_Get_ExpectSameValueAsPassedToConstructor(string verb, string path)
		{
			((object) new Route(verb, path, DummyMethodInfo).Method).Should().BeSameAs(DummyMethodInfo);
		}
	}
}
