using Autofac;
using ContractFirst.Api.Controllers.Bases;
using Mediator.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContractFirst.Api.FilterAndMiddlewares;

public class AutoResolveFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.Controller;
        if (controller is IHasMediator mediatorController)
        {
            var lifetimeScope = context.HttpContext.RequestServices.GetService<ILifetimeScope>();
            if (lifetimeScope != null && lifetimeScope.TryResolve<IMediator>(out var mediator))
            {
                mediatorController.Mediator = mediator;
            }
        }

        return next();
    }
}