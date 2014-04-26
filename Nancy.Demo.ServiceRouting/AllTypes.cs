using System;
using System.Collections.Generic;

namespace Restall.Nancy.Demo.ServiceRouting
{
	public static class AllTypes
	{
		public static IEnumerable<Type> InAssembly
		{
			get { return typeof(AllTypes).Assembly.GetTypes(); }
		}
	}
}
