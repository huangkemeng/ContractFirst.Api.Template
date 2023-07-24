using ContractFirst.Api.Engines.Bases;
using Microsoft.AspNetCore.Builder;

namespace ContractFirst.Api.Engines.SwaggerEngines;

public class UseSwagger : IAppEngine
{
    private readonly WebApplication app;
    private readonly EngineBuilderOptions builderOptions;

    public UseSwagger(WebApplication app, EngineBuilderOptions builderOptions)
    {
        this.app = app;
        this.builderOptions = builderOptions;
    }

    public void Run()
    {
        if (builderOptions.EnableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                typeof(SwaggerApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    var info = f.GetCustomAttributes(typeof(SwaggerGroupInfoAttribute), false)
                        .OfType<SwaggerGroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerEndpoint($"/swagger/{f.Name}/swagger.json",
                        (info != null ? info.Title : f.Name) + "-" + app.Environment.EnvironmentName);
                });
                options.SwaggerEndpoint("/swagger/Ungrouped/swagger.json",
                    "ContractFirst.Api" + "-" + app.Environment.EnvironmentName);
            });
        }
    }
}