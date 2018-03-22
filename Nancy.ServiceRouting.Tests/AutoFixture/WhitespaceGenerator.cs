using System.Collections.Generic;
using System.Linq;
using Ploeh.AutoFixture.Kernel;

namespace Restall.Nancy.ServiceRouting.Tests.AutoFixture
{
	public class WhitespaceGenerator: ISpecimenBuilder
	{
		private const string Whitespace = " \t\v\r\n\f";

		public object Create(object request, ISpecimenContext context)
		{
			if (!request.Equals(typeof(string)))
				return new NoSpecimen(request);

			return new string(PerpetualWhitespace.Take(RandomNumbers.Next(1, 10)).ToArray());
		}

		private static IEnumerable<char> PerpetualWhitespace
		{
			get { while (true) yield return AnyWhitespaceCharacter; }
		}

		private static char AnyWhitespaceCharacter => Whitespace[RandomNumbers.Next(0, Whitespace.Length)];
	}
}
