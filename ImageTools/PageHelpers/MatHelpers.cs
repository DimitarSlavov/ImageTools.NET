using MatBlazor;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageTools.PageHelpers
{
    internal class MatHelpers : IMatHelpers
    {
        public async Task<string> ConvertImageToBase64StringAsync(IMatFileUploadEntry file)
        {
            using (var stream = new MemoryStream())
            {
                await file.WriteToStreamAsync(stream);

                var imageBytes = stream.ToArray();

                return Convert.ToBase64String(imageBytes);
            }
        }

        public async Task<byte[]> ConvertImageToByteArrayAsync(IMatFileUploadEntry file)
        {
            using (var stream = new MemoryStream())
            {
                await file.WriteToStreamAsync(stream);

                return stream.ToArray();
            }
        }
    }
}
