using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tools.Core;
using Tools.Http.Models;

namespace Tools.Http
{
	public static class HttpClientGetRequestExtensions
	{
		public static async Task<Response<TRequestBody>> GetAsync<TRequestBody>(
			this HttpClient httpClient,
			string path)
			where TRequestBody : class
		{
			httpClient.AddDefaultHeaders();

			var url = HttpHelper.NodeUrlFormat(path);

			try
			{
				var httpResponse = await httpClient.GetAsync(url);

				var content = await httpResponse.Content.ReadAsStringAsync();

				if (content == null)
				{
					return Response<TRequestBody>.GetResponse();
				}

				var response = new Response<TRequestBody>();

				try
				{
					var error = JsonHelper.Deserialize<HttpResponseModel>(content);

					if (!(error.Message is null))
					{
						return Response<TRequestBody>.GetResponse(
							error.Message,
							error.Code);
					}
				}
				catch { }

				var model = JsonHelper.Deserialize<TRequestBody>(content);

				response.Content = model;

				return response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
