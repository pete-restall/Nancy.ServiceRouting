using System.Reflection;
using Ploeh.AutoFixture;

namespace Nancy.ServiceRouting.Tests.AutoFixture
{
	public class WhitespaceCustomization: ICustomization
	{
		private readonly ParameterInfo parameter;

		public WhitespaceCustomization(ParameterInfo parameter)
		{
			this.parameter = parameter;
		}

		public void Customize(IFixture fixture)
		{
			fixture.Customizations.Add(new ParameterFilteredSpecimenBuilder(this.parameter, new WhitespaceGenerator()));
		}
	}
}
