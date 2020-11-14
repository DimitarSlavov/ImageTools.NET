using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tools.API.Constants;
using Tools.Core;
using Tools.Core.Models;

namespace Tools.API.Controllers
{
    [ApiController]
    [Route(ControllerConstants.ControllerRoute)]
    public class GraphicsController : CommonControllerBase
    {
        private readonly ILogger<GraphicsController> logger;
        private readonly IDrawingsHelper drawingsHelper;

        public GraphicsController(
            ILogger<GraphicsController> logger,
            IDrawingsHelper drawingsHelper)
        {
            this.logger = logger;
            this.drawingsHelper = drawingsHelper;
        }

        [HttpPost(nameof(GetOriginalImageQualityAsync))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetOriginalImageQualityAsync(
            [FromBody] ImageDetails imageDetails)
        {
            Func<Task<ImageDetails>> content = async () =>
            {
                return await drawingsHelper.OptimizeImageAsync(imageDetails);
            };

            return await TryReturnContentResultAsync(content);
        }

        [HttpPost(nameof(GetImageAsOptimizedBase64StringAsync))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetImageAsOptimizedBase64StringAsync(
            [FromBody] ImageDetails imageDetails)
        {
            Func<Task<string>> content = async () =>
            {
                return await drawingsHelper.ConvertImageToBase64StringOptimizedAsync(imageDetails);
            };

            return await TryReturnContentResultAsync(content);
        }

        [HttpPost(nameof(GetImageListAsOptimizedBase64StringAsync))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetImageListAsOptimizedBase64StringAsync(
            [FromBody] IEnumerable<ImageDetails> imageDetailsCollection)
        {
            Func<Task<List<string>>> content = async () =>
            {
                var result = new List<string>();

                foreach (var imageDetails in imageDetailsCollection)
                {
                    var base64 = await drawingsHelper.ConvertImageToBase64StringOptimizedAsync(imageDetails);
                    result.Add(base64);
                }

                return result;
            };

            return await TryReturnContentResultAsync(content);
        }

        [HttpPost(nameof(CreateSvgFileAsync))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateSvgFileAsync(
            [FromBody] byte[] imageBytes)
        {
            Func<Task<string>> content = async () =>
            {
                var webRootFiles = Path.Combine(Startup.ContentRootPath, ControllerConstants.WebRootFiles);

                return  await drawingsHelper.ConvertImageToSvgAsync(imageBytes, webRootFiles);
            };

            return await TryReturnContentResultAsync(content);
        }

        [HttpGet(nameof(GetSvgFileAsync))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetSvgFileAsync(
            [FromQuery] string fileName)
        {
            try
            {
                return await ZipContentResultByName(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get file", ex.InnerException);
            }
        }
    }
}
