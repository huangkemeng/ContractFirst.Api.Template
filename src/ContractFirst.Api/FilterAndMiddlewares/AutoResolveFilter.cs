using ContractFirst.Api.Primary.Bases;
using Microsoft.AspNetCore.Mvc.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ContractFirst.Api.FilterAndMiddlewares;

public class AutoResolveFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.Controller;
        var type = controller.GetType();
        var typeProperties = type.GetProperties();
        foreach (var typeProperty in typeProperties)
        {
            if (typeProperty.IsPubliclyWritable() && typeProperty.HasAttribute<AutoResolveAttribute>())
            {
                if (typeProperty.GetValue(controller) == null)
                {
                    if (CurrentApplication.TryContextResolve(typeProperty.PropertyType, out var autoResolvePropertyValue))
                    {
                        typeProperty.SetValue(controller, autoResolvePropertyValue);
                    }
                }
            }
        }
        return next();
    }
}