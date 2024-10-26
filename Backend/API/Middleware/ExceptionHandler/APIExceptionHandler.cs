using System.Net;
using Backend.Cores.Commons;
using Microsoft.AspNetCore.Diagnostics;

namespace Backend.API.Middleware.ExceptionHandler
{
    public class APIExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<APIExceptionHandler> logger = null!; // Need more researching

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            HttpResponseData response = new HttpResponseData
            {
                Message = exception.Message != null ? exception.Message : "Something gone wrong and we don't know (Exception_Unknown)",
                Data = exception.Data,
            };
            
            httpContext.Response.StatusCode = exception.Data.Contains("type") ? DetermineResponseStatusCode((exception.Data["type"] as string)!) : 500;
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }

        private int DetermineResponseStatusCode(string errorType)
        {
            switch(errorType)
            {
                case "Invalid":
                    return 400;
                case "NotFound":
                    return 404;
                case "Unauthorized":
                    return 401;
                case "UnknownError":
                    return 500;
                default:
                    return 200;
            }
        }
    }
}
