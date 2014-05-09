using System;
using System.Threading.Tasks;

namespace Restall.Nancy.ServiceRouting.Tests
{
	public static class LongRunningTask
	{
		public static Task Instance()
		{
			return Task.Delay(TimeSpan.FromMilliseconds(10));
		}

		public static Task<T> Instance<T>()
		{
			return Task.FromResult(default(T));
		}
	}
}
