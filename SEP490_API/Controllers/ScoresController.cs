using Azure.Core;
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
    public class ScoresController : ControllerBase
    {
        private readonly IScoreRepository _scoreRepository;

        public ScoresController(IScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
        }

        [HttpGet("ByClassAllSubject")]
        public async Task<IActionResult> ByClassAllSubject(string className, string schoolYear)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Get Mark")))
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

                return Ok(await _scoreRepository.GetAverageScoresByClass(className, schoolYear));
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

        [HttpGet("ByClassBySubject")]
        public async Task<IActionResult> ByClassBySubject(string className, string subjectName, string schoolYear)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Get Mark")))
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

                return Ok(await _scoreRepository.GetScoresByClassBySubject(className, subjectName, schoolYear));
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

        [HttpGet("ByStudentAllSubject")]
        public async Task<IActionResult> ByStudentAllSubject(string studentID, string schoolYear)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Get Mark")))
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

                ScoreStudentResponse s = await _scoreRepository.GetScoresByStudentAllSubject(studentID, schoolYear);

                return Ok(s);
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

        [HttpGet("ByStudentBySubject")]
        public async Task<IActionResult> ByStudentBySubject(string studentID, string subject, string schoolYear)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Get Mark")))
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

                ScoreStudentResponse s = await _scoreRepository.GetScoresByStudentBySubject(studentID, subject, schoolYear);

                return Ok(s);
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


        [HttpGet("Template")]
        public async Task<IActionResult> ExportToExcel(string className, string schoolYear, string semester, string subjectName, string component, int indexCol = 1)
        {
            try
            {
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


                var fileContent = await _scoreRepository.GenerateExcelFile(className, schoolYear, semester, subjectName, component, indexCol);
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", className.ToLower() + "_" + component.ToLower()+ ".xlsx");
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


        [HttpPost("Excel")]
        public async Task<IActionResult> CreateByExcel([FromForm] ExcelRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Add Mark")))
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

                await _scoreRepository.AddScoreByExcel(accountId, request);

                return Ok("Thêm điểm thành công");
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


        [HttpPut("Excel")]
        public async Task<IActionResult> UpdateByExcel([FromForm] ExcelRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("");
                }

                if (!(User.IsInRole("Admin") || User.IsInRole("Update Mark")))
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

                await _scoreRepository.UpdateScoreByExcel(accountId, request);

                return Ok("Cập nhật điểm thành công");
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
