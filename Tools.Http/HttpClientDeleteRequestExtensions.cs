using System.Net.Http;
using System.Threading.Tasks;
using Tools.Core;
using Tools.Http.Models;

namespace Tools.Http
{
    public static class HttpClientDeleteRequestExtensions
    {
        public static async Task<Response> DeleteAsync(
            this HttpClient httpClient,
            string path)
        {
            httpClient.AddDefaultHeaders();

            var httpResponse = await httpClient.DeleteAsync(HttpHelper.NodeUrlFormat(path));

            var content = await httpResponse.Content.ReadAsStringAsync();

            var response = Response.GetResponse();

            if (!string.IsNullOrEmpty(content))
            {
                var httpResponseModel = JsonHelper.Deserialize<HttpResponseModel>(content);
                response.HttpResponseModel = httpResponseModel;
            }

            return response;
        }

        public static async Task<Response<TResponseContent>> DeleteAsync<TResponseContent, TRequestBody>(
            this HttpClient httpClient,
            string path,
            TRequestBody data)
            where TResponseContent : class
            where TRequestBody : class
        {
            var dataAsString = JsonHelper.Serialize(data);
            var httpContent = new StringContent(dataAsString);

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Delete;
            httpRequestMessage.AddDefaultHeaders();
            httpRequestMessage.Content = httpContent;
            httpRequestMessage.RequestUri = HttpHelper.NodeUrlFormat(path);

            var httpResponse = await httpClient.SendAsync(httpRequestMessage);

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
    }
}
