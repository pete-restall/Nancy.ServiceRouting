namespace Nancy.Demo.ServiceRouting.Api
{
	public class IndexService
	{
		public IndexResponse Welcome(IndexRequest request)
		{
			return new IndexResponse
				{
					AvailableDemos = new[]
						{
							new AvailableDemo { Href = "/greet", Title = "Simple Greeting Request / Response" }
						}
				};
		}
	}
}
