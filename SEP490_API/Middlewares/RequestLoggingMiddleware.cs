using Serilog;

namespace SEP490_API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Log.Information($"Received request: {context.Request.Method} {context.Request.Path}");

            await _next(context);
        }
    }
}
