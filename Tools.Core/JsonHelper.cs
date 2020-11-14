using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools.Core
{
	public static class JsonHelper
	{
		public static async Task<T> DeserializeAsync<T>(Stream stream)
        {
			stream.Position = default(int);

			return await JsonSerializer.DeserializeAsync<T>(stream);
		}

		public static T Deserialize<T>(string content)
        {
			return JsonSerializer.Deserialize<T>(content);
		}

		public static string Serialize<T>(T data)
        {
			return JsonSerializer.Serialize(data);
		}
	}
}
