using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace Tools.Http
{
    internal static class HttpExtensions
    {
		public static HttpRequestHeaders AddOrigin(
			this HttpRequestHeaders httpRequestHeaders,
			string url = "https://localhost:44366")
		{
			httpRequestHeaders.Add("Origin", url);

			return httpRequestHeaders;
		}

		public static HttpClient AddDefaultHeaders(this HttpClient httpClient)
		{
			httpClient.DefaultRequestHeaders
			  .Accept
			  .Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
			httpClient.DefaultRequestHeaders.AddOrigin();

			return httpClient;
		}

		public static HttpRequestMessage AddDefaultHeaders(this HttpRequestMessage httpClient)
		{
			httpClient.Headers
			  .Accept
			  .Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
			httpClient.Headers.AddOrigin();

			return httpClient;
		}
	}
}
