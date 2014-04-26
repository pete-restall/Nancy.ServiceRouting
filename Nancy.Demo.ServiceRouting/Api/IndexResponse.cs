using System.Collections.Generic;

namespace Restall.Nancy.Demo.ServiceRouting.Api
{
	public class IndexResponse
	{
		public IEnumerable<AvailableDemo> AvailableDemos { get; set; }
	}
}
