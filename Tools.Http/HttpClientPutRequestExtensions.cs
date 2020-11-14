using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Tools.Core;
using Tools.Http.Models;

namespace Tools.Http
{
	public static class HttpClientPutRequestExtensions
	{
		public static async Task<Response<TResponseContent>> PutAsync<TResponseContent, TRequestBody>(
			this HttpClient httpClient,
			string path,
			TRequestBody data)
			where TResponseContent : class
			where TRequestBody : class
		{
			httpClient.AddDefaultHeaders();

			var dataAsString = JsonHelper.Serialize(data);
			var httpContent = new StringContent(dataAsString);
			httpContent.Headers.ContentType.MediaType = MediaTypeNames.Application.Json;

			var httpResponse = await httpClient.PutAsync(HttpHelper.NodeUrlFormat(path), httpContent);

			var content = await httpResponse.Content.ReadAsStringAsync();

			var response = Response<TResponseContent>.GetResponse();

			var error = JsonHelper.Deserialize<HttpResponseModel>(content);

			if (error != null)
			{
				response.HttpResponseModel = error;

				return response;
			}

			var model = JsonHelper.Deserialize<TResponseContent>(content);
			response.Content = model;

			return response;
		}

		public static async Task<Response> PutAsync<TRequestBody>(
			this HttpClient httpClient,
			string path,
			TRequestBody data)
			where TRequestBody : class
		{
			httpClient.AddDefaultHeaders();

			var dataAsString = JsonHelper.Serialize(data);
			var httpContent = new StringContent(dataAsString);
			httpContent.Headers.ContentType.MediaType = MediaTypeNames.Application.Json;

			var httpResponse = await httpClient.PutAsync(HttpHelper.NodeUrlFormat(path), httpContent);

			var content = await httpResponse.Content.ReadAsStringAsync();

			var response = Response.GetResponse();

			if (!string.IsNullOrEmpty(content))
			{
				var httpResponseModel = JsonHelper.Deserialize<HttpResponseModel>(content);
				response.HttpResponseModel = httpResponseModel;
			}

			return response;
		}

		public static async Task<Response> PutAsync(
			this HttpClient httpClient,
			string path)
		{
			httpClient.AddDefaultHeaders();

			var httpRequesMessage = new HttpRequestMessage
			{
				RequestUri = HttpHelper.NodeUrlFormat(path),
				Method = HttpMethod.Put
			};

			var httpResponse = await httpClient.SendAsync(httpRequesMessage);

			var content = await httpResponse.Content.ReadAsStringAsync();

			var response = Response.GetResponse();

			if(!string.IsNullOrEmpty(content))
            {
				var httpResponseModel = JsonHelper.Deserialize<HttpResponseModel>(content);

				response.HttpResponseModel = httpResponseModel;
			}
			
			return response;
		}
	}
}
