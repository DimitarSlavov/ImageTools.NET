using System.Threading.Tasks;
using Tools.Core.Models;

namespace Tools.Core
{
    public interface IDrawingsHelper
    {
        Task<string> ConvertImageToBase64StringOptimizedAsync(ImageDetails imageDetails);

        Task<ImageDetails> OptimizeImageAsync(ImageDetails imageDetails);

        Task<string> ConvertImageToSvgAsync(byte[] imageBytes, string webRootFiles);
    }
}
