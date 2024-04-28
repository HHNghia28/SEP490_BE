using SEP490_API.Filter;
using System.Net;

namespace SEP490_API.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            APIResponse<string> aPIResponse = new()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Internal Server Error",
                Success = false,
                Data = "An unexpected error occurred. Please try again later."
            };

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(aPIResponse);

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = StatusCodes.Status200OK;

            
            await context.Response.WriteAsync(jsonResponse);
        }
    }

}
