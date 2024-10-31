using System.Net;
using Backend.Cores.Commons;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace Backend.API.Middleware.ExceptionHandler
{
    public class APIExceptionHandler : IExceptionHandler
    {
        //private readonly ILogger<APIExceptionHandler> logger = null!; // Need more researching

        Dictionary<string, object> DefaultData = new Dictionary<string, object>
        {
            { "error",  "Internal_Exception"},
            { "message", "Sorry for the inconvinience, there might have been some internal errors going on our side, please contact tech support and give them error details" },
            { "detail", "Something gone wrong and we don't know how to responses."},
            { "type", "Unknown"},
            { "value", null!},
        };

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            DefaultData["type"] = exception.GetType().Name;

            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsJsonAsync(DefaultData, cancellationToken);

            return true;
        }

        /// <summary>
        /// Translate error type string into corresponding http status code.
        /// </summary>
        /// <param name="errorType">The name of the error type</param>
        /// <returns>an <see cref="int"/> http code.</returns>
        [Obsolete("This method is not needed due to the utilization of filter attributes")]
        private int DetermineResponseStatusCode(string errorType)
        {
            switch(errorType)
            {
                case "Invalid":
                    return 400;
                case "Unauthorized":
                    return 401;
                case "NotFound":
                    return 404;
                case "NotAcceptable":
                    return 406;
                case "Conflict":
                    return 409;
                case "MediaNotSupported":
                    return 415;
                default:
                    return 500;
            }
        }
    }
}
