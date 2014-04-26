using System;
using Nancy.Testing;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public static class BrowserExtensions
	{
		public static T Get<T>(this Browser browser, string path)
		{
			return browser.Get(path, with => with.AcceptJson()).Body.DeserializeJson<T>();
		}

		public static T Put<T>(this Browser browser, string path, object request)
		{
			return browser.Put(path, FormBasedContextWithBody(request)).Body.DeserializeJson<T>();
		}

		private static Action<BrowserContext> FormBasedContextWithBody(object request)
		{
			return with => with.AcceptJson().JsonBody(request);
		}

		public static T Post<T>(this Browser browser, string path, object request)
		{
			return browser.Post(path, FormBasedContextWithBody(request)).Body.DeserializeJson<T>();
		}

		public static T Patch<T>(this Browser browser, string path, object request)
		{
			return browser.Patch(path, FormBasedContextWithBody(request)).Body.DeserializeJson<T>();
		}

		public static T Delete<T>(this Browser browser, string path)
		{
			return browser.Delete(path, with => with.AcceptJson()).Body.DeserializeJson<T>();
		}

		public static T Options<T>(this Browser browser, string path)
		{
			return browser.Options(path, with => with.AcceptJson()).Body.DeserializeJson<T>();
		}
	}
}
