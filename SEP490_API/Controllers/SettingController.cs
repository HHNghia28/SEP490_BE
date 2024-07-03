using BusinessObject.DTOs;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SEP490_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingRepository _settingRepository;

        public SettingController(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetSetting()
        {
            try
            {

                return Ok(await _settingRepository.GetSetting());
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

        [HttpPut]
        public async Task<IActionResult> CreateSetting(SettingRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Update Setting")))
                {
                    return new ObjectResult("")
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                }

                string accountId = User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;

                if (string.IsNullOrEmpty(accountId))
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

                await _settingRepository.UpdateSetting(accountId, request);

                return Ok("Thêm môn học thành công");
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
