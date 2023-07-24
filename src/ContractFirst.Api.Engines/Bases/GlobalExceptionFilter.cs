using ContractFirst.Api.Realization.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContractFirst.Api.Engines.Bases;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        if (exception is BusinessException businessException)
        {
            if (businessException.Type == BusinessExceptionTypeEnum.UnauthorizedIdentity)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new UnauthorizedObjectResult(new
                {
                    Error = businessException.TypeName, businessException.Message
                });
            }
            else if (businessException.Type == BusinessExceptionTypeEnum.Forbidden)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Result = new ForbidResult();
            }
            else
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new BadRequestObjectResult(new
                {
                    Error = businessException.TypeName, businessException.Message
                });
            }
        }
    }
}