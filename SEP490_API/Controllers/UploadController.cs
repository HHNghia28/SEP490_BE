using Azure.Core;
using BusinessObject.DTOs;
using BusinessObject.Exceptions;
using BusinessObject.IServices;
using DataAccess.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SEP490_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IImageService _imageService;

        public UploadController(IImageService imageService)
        {
            this._imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] ImageRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                    return BadRequest(errors);
                }

                string folder = string.IsNullOrEmpty(request.Folder) ? "Images" : request.Folder;
                string name = string.IsNullOrEmpty(request.Name) ? null : request.Name;
                var imageUrl = await _imageService.UploadImage(request.File, folder, name);

                return Ok(imageUrl);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
