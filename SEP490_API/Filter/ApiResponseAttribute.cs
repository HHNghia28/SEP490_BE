using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace SEP490_API.Filter
{
    public class ApiResponseAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var statusCode = objectResult.StatusCode ?? 200;
                var success = statusCode >= 200 && statusCode < 300;
                var message = GetMessageByStatusCode(statusCode);

                var apiResponse = new APIResponse<object>
                {
                    Success = success,
                    Message = message,
                    StatusCode = statusCode,
                    Data = objectResult.Value
                };

                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        private string GetMessageByStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 100:
                    return "Continue";
                case 101:
                    return "Switching Protocols";
                case 200:
                    return "OK";
                case 201:
                    return "Created";
                case 202:
                    return "Accepted";
                case 203:
                    return "Non-Authoritative Information";
                case 204:
                    return "No Content";
                case 205:
                    return "Reset Content";
                case 206:
                    return "Partial Content";
                case 300:
                    return "Multiple Choices";
                case 301:
                    return "Moved Permanently";
                case 302:
                    return "Found";
                case 303:
                    return "See Other";
                case 304:
                    return "Not Modified";
                case 305:
                    return "Use Proxy";
                case 307:
                    return "Temporary Redirect";
                case 400:
                    return "Bad Request";
                case 401:
                    return "Unauthorized";
                case 402:
                    return "Payment Required";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 406:
                    return "Not Acceptable";
                case 407:
                    return "Proxy Authentication Required";
                case 408:
                    return "Request Timeout";
                case 409:
                    return "Conflict";
                case 410:
                    return "Gone";
                case 411:
                    return "Length Required";
                case 412:
                    return "Precondition Failed";
                case 413:
                    return "Payload Too Large";
                case 414:
                    return "URI Too Long";
                case 415:
                    return "Unsupported Media Type";
                case 416:
                    return "Range Not Satisfiable";
                case 417:
                    return "Expectation Failed";
                case 500:
                    return "Internal Server Error";
                case 501:
                    return "Not Implemented";
                case 502:
                    return "Bad Gateway";
                case 503:
                    return "Service Unavailable";
                case 504:
                    return "Gateway Timeout";
                case 505:
                    return "HTTP Version Not Supported";
                default:
                    return "Unknown Status";
            }
        }

    }
}
