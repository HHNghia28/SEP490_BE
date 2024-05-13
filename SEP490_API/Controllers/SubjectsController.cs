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
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectsController(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Get Subject")))
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

                return Ok(await _subjectRepository.GetSubjects());
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

        [HttpGet("{subjectID}")]
        public async Task<IActionResult> Get(string subjectID)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Get Subject")))
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

                return Ok(await _subjectRepository.GetSubject(subjectID));
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
        public async Task<IActionResult> Create(SubjectRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Add Subject")))
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

                await _subjectRepository.AddSubject(accountId, request);

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

        [HttpDelete("{subjectID}")]
        public async Task<IActionResult> Delete(string subjectID)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Delete Subject")))
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

                await _subjectRepository.DeleteSubject(accountId, subjectID);

                return Ok("Xóa môn học thành công");
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

        [HttpPut("{subjectID}")]
        public async Task<IActionResult> Update(string subjectID,[FromBody] SubjectRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Update Subject")))
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

                await _subjectRepository.UpdateSubject(accountId, subjectID, request);

                return Ok("Chỉnh sửa môn học thành công");
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
