using System;
using Nancy;

namespace Restall.Nancy.ServiceRouting.Tests.Unit
{
	public static class StubNancyModelBinding
	{
		[ThreadStatic]
		private static object stubBoundModel;

		[ThreadStatic]
		private static INancyModule spiedModule;

		public static TModel StubNancyModelBind<TModel>(this INancyModule module)
		{
			SpiedModule = module;
			return (TModel) StubBoundModel;
		}

		public static object StubBoundModel
		{
			get { return stubBoundModel; }
			set { stubBoundModel = value; }
		}

		public static INancyModule SpiedModule
		{
			get { return spiedModule; }
			set { spiedModule = value; }
		}
	}
}
