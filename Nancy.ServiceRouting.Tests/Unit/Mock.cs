using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public static class Mock
	{
		public static IEnumerable<T> Enumerable<T>()
		{
			var enumerable = MockRepository.GenerateMock<IEnumerable<T>>();
			enumerable.Expect(x => x.GetEnumerator()).Do(new Func<IEnumerator<T>>(new T[0].AsEnumerable().GetEnumerator));
			return enumerable;
		}
	}
}
