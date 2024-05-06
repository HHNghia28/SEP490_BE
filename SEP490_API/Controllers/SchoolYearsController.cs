using Azure.Core;
using BusinessObject.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SEP490_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolYearsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //{
                //    return Unauthorized("");
                //}

                //if (!User.IsInRole("GetSchoolYears"))
                //{
                //    return new ObjectResult("")
                //    {
                //        StatusCode = StatusCodes.Status403Forbidden
                //    };
                //}

                string accountId = User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;

                return Ok(accountId);
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
