using System;
using Ploeh.AutoFixture.Kernel;

namespace Restall.Nancy.ServiceRouting.Tests.AutoFixture
{
	public class RandomIntegerInclusiveRangeGenerator: ISpecimenBuilder
	{
		private readonly int min;
		private readonly int max;

		public RandomIntegerInclusiveRangeGenerator(int min, int max)
		{
			this.min = Math.Min(min, max);
			this.max = Math.Max(min, max);
		}

		public object Create(object request, ISpecimenContext context)
		{
			var valueType = request as Type;
			if (valueType == null || !IsAnIntegerType(valueType))
				return new NoSpecimen(request);

			return Convert.ChangeType(RandomNumbers.Next(this.min, this.max == int.MaxValue ? int.MaxValue : this.max + 1), valueType);
		}

		private static bool IsAnIntegerType(Type valueType)
		{
			return
				valueType == typeof(sbyte) || valueType == typeof(byte) ||
				valueType == typeof(short) || valueType == typeof(ushort) ||
				valueType == typeof(int) || valueType == typeof(uint) ||
				valueType == typeof(long) || valueType == typeof(ulong);
		}
	}
}
