using Application.Interface;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IConfiguration _config;

        public ImageController(IImageService imageService, IConfiguration config)
        {
            _imageService = imageService;
            _config = config;
        }

        // POST /api/image/upload
        [HttpPost("images")]
        public async Task<IActionResult> Upload(
            IFormFile file,
            CancellationToken ct)
        {
            var result = await _imageService.UploadAsync(
                file.OpenReadStream(),
                file.ContentType,
                ct);

            return Ok(new { imageName = result.Value });
        }
    }
}
