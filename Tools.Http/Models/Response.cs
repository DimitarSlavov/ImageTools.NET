using System;
using System.Net;
using System.Threading.Tasks;

namespace Tools.Http.Models
{
	public class Response<T> : Response
	{
        public Response() { }

        public Response(T content, string message, HttpStatusCode code)
        {
			Content = content;
			HttpResponseModel = new HttpResponseModel
			{
				Message = message,
				Code = code
			};
		}

		public T Content { get; set; }

		public static Response<T> TryGetResult(Func<T> func)
		{
			try
			{
				var content = func.Invoke();

				return GetContentResponse(content, HttpStatusCode.OK);
			}
			catch (Exception ex)
			{
				return GetResponse(ex.Message, HttpStatusCode.BadRequest);
			}
		}

		public static async Task<Response<T>> TryGetResultAsync(Func<Task<T>> func)
		{
			try
			{
				var content = await func.Invoke();

				return GetContentResponse(content, HttpStatusCode.OK);
			}
			catch (Exception ex)
			{
				return GetResponse(ex.Message, HttpStatusCode.BadRequest);
			}
		}

		public static Response<T> GetContentResponse(T content, string message, HttpStatusCode code)
        {
			return new Response<T>(content, message, code);
		}

		public static Response<T> GetContentResponse(T content, HttpStatusCode code)
		{
			return new Response<T>(content, code.ToString(), code);
		}

		new public static Response<T> GetResponse(string message, HttpStatusCode code)
        {
			var httpResponseModel = new HttpResponseModel
			{
				Code = code,
				Message = message
			};

			var response = new Response<T>()
			{
				HttpResponseModel = httpResponseModel
			};

			return response;
		}

		new public static Response<T> GetResponse()
		{
			return new Response<T>();
		}
	}

	public class Response
	{
		public HttpResponseModel HttpResponseModel { get; set; }

		public static Response GetResponse(string message, HttpStatusCode code)
        {
			var httpResponseModel = new HttpResponseModel
			{
				Code = code,
				Message = message
			};

			var response = new Response()
			{
				HttpResponseModel = httpResponseModel
			};

			return response;
		}

		public static Response GetResponse()
		{
			return new Response();
		}
	}
}
