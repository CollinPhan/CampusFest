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
                // Swtiching status code base on type of the error in the exception.
                switch (context.Exception.Data["type"])
                {
                    case "invalid":
                        context.HttpContext.Response.StatusCode = 400;
                        break;
                    case "unauthorized":
                        context.HttpContext.Response.StatusCode = 401;
                        break;
                    case "notpermitted":
                        context.HttpContext.Response.StatusCode = 403;
                        break;
                    case "notfound":
                        context.HttpContext.Response.StatusCode = 404;
                        break;
                    case "unacceptable":
                        context.HttpContext.Response.StatusCode = 406;
                        break;
                    case "conflict":
                        context.HttpContext.Response.StatusCode = 409;
                        break;
                    case "teapot":
                        context.HttpContext.Response.StatusCode = 418;
                        break;
                    default:
                        break;
                }

                // Return error
                context.Result = new ObjectResult(context.Exception.Data) { StatusCode = 400 };
            }
           
        }
    }
}
