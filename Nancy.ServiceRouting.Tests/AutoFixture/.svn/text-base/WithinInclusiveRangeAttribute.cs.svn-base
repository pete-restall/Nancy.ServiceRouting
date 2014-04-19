using System.Reflection;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace Nancy.ServiceRouting.Tests.AutoFixture
{
	public class WithinInclusiveRangeAttribute: CustomizeAttribute
	{
		private readonly int min;
		private readonly int max;

		public WithinInclusiveRangeAttribute(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public override ICustomization GetCustomization(ParameterInfo parameter)
		{
			return new RandomIntegerInclusiveRangeCustomization(parameter, this.min, this.max);
		}
	}
}
