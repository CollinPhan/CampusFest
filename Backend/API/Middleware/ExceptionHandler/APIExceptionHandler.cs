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
                Message = exception.Message != null ? exception.Message : exception.ToString().Split(Environment.NewLine).First(),
                Data = exception.Data,
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}
