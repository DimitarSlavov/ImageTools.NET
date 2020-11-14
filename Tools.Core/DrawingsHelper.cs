using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tools.Core.Models;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Tools.Core
{
    internal class DrawingsHelper : IDrawingsHelper
    {
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].MimeType == mimeType)
                {
                    return codecs[i];
                }
            }

            return null;
        }

        private static EncoderParameters GetEncoderParameters(int quality)
        {
            var encoder = Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            var encoderParameter = new EncoderParameter(encoder, quality);

            encoderParameters.Param[0] = encoderParameter;

            return encoderParameters;
        }

        private static async Task GenerateSvgContentAsync(Bitmap bitmap, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                var svgOpeningTag = $"<svg height='{bitmap.Height}' width='{bitmap.Width}' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink'>";
                var svgLine = "<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' style='stroke:rgb({4},{5},{5});stroke-width:1'/>";
                var svgClosingTag = "</svg>";

                await sw.WriteAsync(svgOpeningTag);

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width - 1; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        var svgLineFormat = string.Format(svgLine, x, y, x + 1, y, color.R, color.G, color.B);

                        await sw.WriteAsync(svgLineFormat);
                    }
                }

                await sw.WriteAsync(svgClosingTag);
            }
        }

        private static string GetFilePath(string webRootFiles, string fileName)
        {
            if (!Directory.Exists(webRootFiles))
            {
                Directory.CreateDirectory(webRootFiles);
            }

            var filePath = Path.Combine(webRootFiles, fileName);

            return filePath;
        }

        private static async Task<string> CreateSvgFileAsync(Bitmap bitmap, string webRootFiles)
        {
            var fileName = $@"image-{DateTime.Now.Ticks}.svg";
            var filePath = GetFilePath(webRootFiles, fileName);

            await GenerateSvgContentAsync(bitmap, filePath);

            return fileName;
        }

        public async Task<string> ConvertImageToBase64StringOptimizedAsync(ImageDetails imageDetails)
        {
            return await Task.Run(() =>
            {
                using (var inputMs = new MemoryStream(imageDetails.Data))
                using (var image = Image.FromStream(inputMs))
                {
                    var imageCodecInfo = GetEncoderInfo(imageDetails.Type);
                    var encoderParameters = GetEncoderParameters(imageDetails.Quality);

                    using (var outputMs = new MemoryStream())
                    {
                        image.Save(outputMs, imageCodecInfo, encoderParameters);
                        var imageBytes = outputMs.ToArray();

                        return Convert.ToBase64String(imageBytes);
                    }
                }
            });
        }

        private async Task<ImageDetails> GetImageDataAsync(ImageDetails imageDetails, int quality)
        {
            return await Task.Run(() =>
            {
                using (var inputMs = new MemoryStream(imageDetails.Data))
                using (var image = Image.FromStream(inputMs))
                {
                    var imageCodecInfo = GetEncoderInfo(imageDetails.Type);
                    var encoderParameters = GetEncoderParameters(quality);

                    using (var outputMs = new MemoryStream())
                    {
                        image.Save(outputMs, imageCodecInfo, encoderParameters);
                        var imageBytes = outputMs.ToArray();

                        var newImageDetails = new ImageDetails
                        {
                            Data = imageBytes,
                            Size = imageBytes.Length,
                            Quality = quality
                        };

                        return newImageDetails;
                    }
                }
            });
        }

        private async Task<ImageDetails> GetImageDataAsync(ImageDetails imageDetails)
        {
            return await Task.Run(() =>
            {
                using (var inputMs = new MemoryStream(imageDetails.Data))
                using (var image = Image.FromStream(inputMs))
                {
                    var imageCodecInfo = GetEncoderInfo(imageDetails.Type);
                    var encoderParameters = GetEncoderParameters(imageDetails.Quality);

                    using (var outputMs = new MemoryStream())
                    {
                        image.Save(outputMs, imageCodecInfo, encoderParameters);
                        var imageBytes = outputMs.ToArray();

                        imageDetails.Data = imageBytes;
                        imageDetails.Size = imageBytes.Length;

                        return imageDetails;
                    }
                }
            });
        }

        public async Task<string> ConvertImageToSvgAsync(byte[] data, string webRootFiles)
        {
            using (var inputMs = new MemoryStream(data))
            using (var bitmap = new Bitmap(inputMs))
            {
                return await CreateSvgFileAsync(bitmap, webRootFiles);
            }
        }

        //TODO: to finish method
        public async Task GetThumbnailImage(byte[] data, string webRootFiles) 
        {
            await Task.Run(() =>
            {
                using (var inputMs = new MemoryStream(data))
                using (var image = Image.FromStream(inputMs))
                {
                    image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
                }
            });
        }

        public async Task<ImageDetails> OptimizeImageAsync(ImageDetails imageDetails)
        {
            var calculatedList = new List<ImageDetails>();

            for (int i = 100; i >= 0; i-=5)
            {
                var currentCalculated = await GetImageDataAsync(imageDetails, i);

                var lastCalculated = default(ImageDetails);

                if (calculatedList.Any())
                {
                    lastCalculated = calculatedList.Last();
                }

                calculatedList.Add(currentCalculated);

                if (imageDetails.Size < currentCalculated.Size)
                {
                    continue;
                }

                if(lastCalculated != null)
                {
                    var lowerQualityLimit = currentCalculated.Quality;
                    var upperQualityLimit = lastCalculated.Quality;

                    for (int j = upperQualityLimit; j >= lowerQualityLimit; j--)
                    {
                        currentCalculated = await GetImageDataAsync(imageDetails, j);

                        calculatedList.Add(currentCalculated);

                        if (imageDetails.Size < currentCalculated.Size)
                        {
                            continue;
                        }

                        break;
                    }
                }

                imageDetails.Quality = currentCalculated.Quality;

                break;
            }

            return imageDetails;
        }
    }
}
