﻿using System.Reflection;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace Restall.Nancy.ServiceRouting.Tests.AutoFixture
{
	public class WhitespaceAttribute: CustomizeAttribute
	{
		public override ICustomization GetCustomization(ParameterInfo parameter)
		{
			return new WhitespaceCustomization(parameter);
		}
	}
}
