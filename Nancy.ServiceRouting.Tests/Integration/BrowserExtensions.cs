using System;
using Nancy.Testing;

namespace Restall.Nancy.ServiceRouting.Tests.Integration
{
	public static class BrowserExtensions
	{
		public static T SendUrlRequest<T>(this Browser browser, string verb, string path)
		{
			switch (verb.ToUpperInvariant())
			{
				case "GET":
					return browser.Get<T>(path);

				case "DELETE":
					return browser.Delete<T>(path);

				case "OPTIONS":
					return browser.Options<T>(path);

				default:
					throw new ArgumentException("Verb " + verb + " not applicable for this method", nameof(verb));
			}
		}

		public static T Get<T>(this Browser browser, string path)
		{
			return browser.Get(path, with => with.AcceptJson()).Body.DeserializeJson<T>();
		}

		private static T Delete<T>(this Browser browser, string path)
		{
			return browser.Delete(path, with => with.AcceptJson()).Body.DeserializeJson<T>();
		}

		private static T Options<T>(this Browser browser, string path)
		{
			return browser.Options(path, with => with.AcceptJson()).Body.DeserializeJson<T>();
		}

		public static T SendFormRequest<T>(this Browser browser, string verb, string path, object request)
		{
			switch (verb.ToUpperInvariant())
			{
				case "PUT":
					return browser.Put<T>(path, request);

				case "POST":
					return browser.Post<T>(path, request);

				case "PATCH":
					return browser.Patch<T>(path, request);

				default:
					throw new ArgumentException("Verb " + verb + " not applicable for this method", nameof(verb));
			}
		}

		private static T Put<T>(this Browser browser, string path, object request)
		{
			return browser.Put(path, FormBasedContextWithBody(request)).Body.DeserializeJson<T>();
		}

		private static Action<BrowserContext> FormBasedContextWithBody(object request)
		{
			return with => with.AcceptJson().JsonBody(request);
		}

		private static T Post<T>(this Browser browser, string path, object request)
		{
			return browser.Post(path, FormBasedContextWithBody(request)).Body.DeserializeJson<T>();
		}

		private static T Patch<T>(this Browser browser, string path, object request)
		{
			return browser.Patch(path, FormBasedContextWithBody(request)).Body.DeserializeJson<T>();
		}
	}
}
