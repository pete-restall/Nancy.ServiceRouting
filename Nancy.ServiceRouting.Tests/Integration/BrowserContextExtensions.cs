﻿using Nancy.Testing;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public static class BrowserContextExtensions
	{
		public static BrowserContext AcceptJson(this BrowserContext context)
		{
			context.Accept("application/json");
			return context;
		}
	}
}
