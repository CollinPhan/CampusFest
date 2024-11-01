using Backend.Cores.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Backend.API.Filters
{
    public class ActionExceptionFilter: ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            // Only handle exceptions that inherits from BaseServiceException
            if (context.Exception is BaseServiceException)
            {
                int status_code = 400;

                // Swtiching status code base on type of the error in the exception.
                switch (context.Exception.Data["type"])
                {
                    case "Invalid":
                        status_code = 400;
                        break;
                    case "Unauthorized":
                        status_code = 401;
                        break;
                    case "NotPermitted":
                        status_code = 403;
                        break;
                    case "NotFound":
                        status_code = 404;
                        break;
                    case "Unacceptable":
                        status_code = 406;
                        break;
                    case "Conflict":
                        status_code = 409;
                        break;
                    case "Teapot":
                        status_code = 418;
                        break;
                    default:
                        break;
                }

                // Return error
                context.Result = new ObjectResult(context.Exception.Data) { StatusCode = status_code };
            }
           
        }
    }
}
