using System.Reflection;
using Ploeh.AutoFixture.Kernel;

namespace Nancy.ServiceRouting.Tests.AutoFixture
{
	public class ParameterFilteredSpecimenBuilder: ISpecimenBuilder
	{
		private readonly ParameterInfo parameter;
		private readonly ISpecimenBuilder typeFilteredBuilder;

		public ParameterFilteredSpecimenBuilder(ParameterInfo parameter, ISpecimenBuilder typeFilteredBuilder)
		{
			this.parameter = parameter;
			this.typeFilteredBuilder = typeFilteredBuilder;
		}

		public object Create(object request, ISpecimenContext context)
		{
			var requestAsParameter = request as ParameterInfo;
			if (requestAsParameter == null || !requestAsParameter.Equals(this.parameter))
				return new NoSpecimen();

			return this.typeFilteredBuilder.Create(this.parameter.ParameterType, context);
		}
	}
}
