using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Tools.Core;
using Tools.Http.Models;

namespace Tools.Http
{
	public static class HttpClientPostRequestExtensions
	{
		public static async Task<Response<TResponseContent>> PostAsync<TResponseContent, TRequestBody>(
			this HttpClient httpClient,
			string path,
			TRequestBody data,
			string requestContentType = null)
		{
			httpClient.AddDefaultHeaders();

			HttpContent httpContent;

            try
            {
				if (typeof(TRequestBody) == typeof(string))
				{
					httpContent = new StringContent(data as string);
				}
				else
				{
					var stringContent = JsonHelper.Serialize(data);
					httpContent = new StringContent(stringContent);
				}

				if (!string.IsNullOrEmpty(requestContentType))
				{
					if (httpContent.Headers?.ContentType?.MediaType != null)
					{
						httpContent.Headers.ContentType.MediaType = requestContentType;
					}
					else
					{
						httpContent.Headers.ContentType = new MediaTypeHeaderValue(requestContentType);
					}
				}
				else
				{
					httpContent.Headers.ContentType.MediaType = MediaTypeNames.Application.Json;
				}

				var httpResponse = await httpClient.PostAsync(HttpHelper.NodeUrlFormat(path), httpContent);

				var content = await httpResponse.Content.ReadAsStringAsync();

				if (content is null)
				{
					return Response<TResponseContent>.GetResponse();
				}

				var response = JsonHelper.Deserialize<Response<TResponseContent>>(content);

				return response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public static async Task<Response<byte[]>> PostAsync(
			this HttpClient httpClient,
			string path,
			byte[] data)
		{
			httpClient.AddDefaultHeaders();

			HttpContent httpContent;

			try
			{
				var stringContent = JsonHelper.Serialize(data);
				httpContent = new StringContent(stringContent);

				httpContent.Headers.ContentType.MediaType = MediaTypeNames.Application.Json;

				var httpResponse = await httpClient.PostAsync(HttpHelper.NodeUrlFormat(path), httpContent);

				var content = await httpResponse.Content.ReadAsByteArrayAsync();

				if (content is null)
				{
					return Response<byte[]>.GetResponse();
				}

				var response = new Response<byte[]>();

				response.Content = content;

				return response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public static async Task<Response> PostAsync<TRequestBody>(
			this HttpClient httpClient,
			string path,
			TRequestBody data)
		{
			httpClient.AddDefaultHeaders();

			HttpContent httpContent;

			try
			{
				if (typeof(TRequestBody) == typeof(string))
				{
					httpContent = new StringContent(data as string);
				}
				else
				{
					var stringContent = JsonHelper.Serialize(data);
					httpContent = new StringContent(stringContent);
				}

				httpContent.Headers.ContentType.MediaType = MediaTypeNames.Application.Json;

				var httpResponse = await httpClient.PostAsync(HttpHelper.NodeUrlFormat(path), httpContent);

				var content = await httpResponse.Content.ReadAsStringAsync();

				if (content is null)
				{
					return Response.GetResponse();
				}

				var response = new Response();

				response.HttpResponseModel = JsonHelper.Deserialize<HttpResponseModel>(content);

				return response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
