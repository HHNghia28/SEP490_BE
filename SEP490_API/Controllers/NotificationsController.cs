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
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationsController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotification()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Get Notification")))
                {
                    return new ObjectResult("")
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
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

                return Ok(await _notificationRepository.GetAllNotifications());
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

        [HttpGet("{notiID}")]
        public async Task<IActionResult> GetNotification(string notiID)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Get Notification")))
                {
                    return new ObjectResult("")
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
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

                return Ok(await _notificationRepository.GetNotification(notiID));
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

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromForm] NotificationRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Add Notification")))
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

                await _notificationRepository.AddNotification(accountId, request);

                return Ok("Thêm thông báo thành công");
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

        [HttpDelete("{notiID}")]
        public async Task<IActionResult> DeleteNotification(string notiID)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Delete Notification")))
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

                await _notificationRepository.DeleteNotification(accountId, notiID);

                return Ok("Xóa thông báo thành công");
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

        [HttpPut("{notiID}")]
        public async Task<IActionResult> UpdateNotification(string notiID, [FromForm] NotificationRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Update Notification")))
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

                await _notificationRepository.UpdateNotification(accountId, notiID, request);

                return Ok("Chỉnh sửa thông báo thành công");
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
