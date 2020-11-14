using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Mime;
using System.Threading.Tasks;
using Tools.API.Constants;
using Tools.Core;
using Tools.Core.Constants;
using Tools.Http.Models;

namespace Tools.API.Controllers
{
    public abstract class CommonControllerBase : ControllerBase
    {
        private async Task<FileContentResult> GetSvgFile(string filePath)
        {
            using (var ms = new MemoryStream())
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(ms);
                return File(ms.ToArray(), MediaTypeNamesExtensions.Image.Svg);
            }
        }

        protected virtual async Task<FileContentResult> FileContentResultByPath(string filePath)
        {
            return await GetSvgFile(filePath);
        }

        protected virtual async Task<FileContentResult> FileContentResultByName(string fileName)
        {
            var filePath = Path.Combine(Startup.ContentRootPath, ControllerConstants.WebRootFiles, fileName);

            return await GetSvgFile(filePath);
        }

        protected virtual async Task<FileContentResult> ZipContentResultByName(string fileName)
        {
            return await Task.Run(() => 
            {
                var filePath = Path.Combine(Startup.ContentRootPath, ControllerConstants.WebRootFiles, fileName);
                var zipFileName = Path.GetFileNameWithoutExtension(fileName) + FileExtensions.Zip;

                using (var ms = new MemoryStream())
                using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    zipArchive.CreateEntryFromFile(filePath, fileName);

                    return File(ms.ToArray(), MediaTypeNames.Application.Zip, zipFileName);
                }
            });
        }

        protected async Task<ContentResult> TryReturnContentResultAsync<T>(Func<Task<T>> func)
        {
            var result = await Response<T>.TryGetResultAsync(func);

            return Content(JsonHelper.Serialize(result));
        }
    }
}
