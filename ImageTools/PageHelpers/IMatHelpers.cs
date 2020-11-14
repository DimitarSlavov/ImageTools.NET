using MatBlazor;
using System.Threading.Tasks;

namespace ImageTools.PageHelpers
{
    internal interface IMatHelpers
    {
        Task<string> ConvertImageToBase64StringAsync(IMatFileUploadEntry file);

        Task<byte[]> ConvertImageToByteArrayAsync(IMatFileUploadEntry file);
    }
}