using System;
using System.Reflection;
using FluentAssertions;
using Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit.Extensions;

namespace Nancy.ServiceRouting.Tests.Unit
{
	public class ServiceRouteTest
	{
		private class StubService
		{
			public object ServiceMethod(object request) { return new object(); }
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullVerb_ExpectArgumentNullExceptionWithCorrectParamName(string path)
		{
			Action constructor = () => new ServiceRoute(null, path, DummyMethodInfo);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("verb");
		}

		private static MethodInfo DummyMethodInfo
		{
			get { return InfoOf.Method<StubService>(x => x.ServiceMethod(null)); }
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullPath_ExpectArgumentNullExceptionWithCorrectParamName(string verb)
		{
			Action constructor = () => new ServiceRoute(verb, null, DummyMethodInfo);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("path");
		}

		[Theory, NancyAutoData]
		public void Constructor_CalledWithNullMethod_ExpectArgumentNullExceptionWithCorrectParamName(string verb, string path)
		{
			Action constructor = () => new ServiceRoute(verb, path, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("method");
		}

		[Theory, NancyAutoData]
		public void Verb_Get_ExpectSameValueAsPassedToConstructor(string verb, string path)
		{
			new ServiceRoute(verb, path, DummyMethodInfo).Verb.Should().Be(verb);
		}

		[Theory, NancyAutoData]
		public void Path_Get_ExpectSameValueAsPassedToConstructor(string verb, string path)
		{
			new ServiceRoute(verb, path, DummyMethodInfo).Path.Should().Be(path);
		}

		[Theory, NancyAutoData]
		public void Method_Get_ExpectSameValueAsPassedToConstructor(string verb, string path)
		{
			((object) new ServiceRoute(verb, path, DummyMethodInfo).Method).Should().BeSameAs(DummyMethodInfo);
		}
	}
}
