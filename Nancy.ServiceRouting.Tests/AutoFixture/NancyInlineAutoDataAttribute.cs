using Ploeh.AutoFixture.Xunit;

namespace Restall.Nancy.ServiceRouting.Tests.AutoFixture
{
	public class NancyInlineAutoDataAttribute: InlineAutoDataAttribute
	{
		public NancyInlineAutoDataAttribute(params object[] values): base(new NancyAutoDataAttribute(), values)
		{
		}
	}
}
