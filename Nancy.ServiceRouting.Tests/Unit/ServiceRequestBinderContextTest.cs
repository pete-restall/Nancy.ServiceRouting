using System;
using FluentAssertions;
using Nancy;
using Restall.Nancy.ServiceRouting.Tests.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public class ServiceRequestBinderContextTest
	{
		[Fact]
		public void Constructor_CalledWithNullNancyModule_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new ServiceRequestBinderContext(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("nancyModule");
		}

		[Theory, NancyAutoData]
		public void NancyModule_Get_ExpectSameValueAsPassedToConstructor(NancyModule nancyModule)
		{
			new ServiceRequestBinderContext(nancyModule).NancyModule.Should().BeSameAs(nancyModule);
		}
	}
}
