using System.Reflection;
using Ploeh.AutoFixture;

namespace Nancy.ServiceRouting.Tests.AutoFixture
{
	public class RandomIntegerInclusiveRangeCustomization: ICustomization
	{
		private readonly ParameterInfo parameter;
		private readonly int min;
		private readonly int max;

		public RandomIntegerInclusiveRangeCustomization(ParameterInfo parameter, int min, int max)
		{
			this.parameter = parameter;
			this.min = min;
			this.max = max;
		}

		public void Customize(IFixture fixture)
		{
			fixture.Customizations.Add(new ParameterFilteredSpecimenBuilder(this.parameter, new RandomIntegerInclusiveRangeGenerator(this.min, this.max)));
		}
	}
}
