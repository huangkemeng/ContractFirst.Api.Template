using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ContractFirst.Api.Engines.Bases;

public static class EngineHub
{
    public static WebApplication BuildWithEngines(this WebApplicationBuilder builder,
        Action<EngineBuilderOptions>? configureOptions = null)
    {
        EngineBuilderOptions engineBuilderOptions = new EngineBuilderOptions();
        configureOptions?.Invoke(engineBuilderOptions);
        builder.Services.AddSingleton(engineBuilderOptions);
        WebApplication? app = null;
        builder.RegisterEngines();
        builder.StartBuilderEngines();
        builder.Services.AddSingleton(services =>
        {
            app ??= builder.Build();
            return app;
        });
        app = builder.Build();
        app.StartAppEngines();
        return app;
    }

    private static void RegisterEngines(this WebApplicationBuilder builder)
    {
        var iEngineType = typeof(IEngine);
        var engineTypes = iEngineType
            .Assembly
            .ExportedTypes
            .Where(e => e.GetInterfaces().Contains(iEngineType) && e.IsClass && !e.IsAbstract)
            .ToList();

        foreach (var engineType in engineTypes)
        {
            var iBaseType = engineType.GetInterfaces()
                .FirstOrDefault(e => e.GetInterfaces().Contains(iEngineType) && e != iEngineType);
            if (iBaseType != null)
            {
                builder.Services.AddSingleton(iBaseType, engineType);
            }
        }
    }

    private static void StartBuilderEngines(this WebApplicationBuilder builder)
    {
        builder.Host.UseServiceProviderFactory(new CustomAutofacServiceProviderFactory())
            .ConfigureServices((_, serviceBuilder) => { serviceBuilder.AddSingleton(serviceBuilder); })
            .ConfigureContainer<ContainerBuilder>((_, container) =>
            {
                builder.Services.AddSingleton(container);
                var services = builder.Services.BuildServiceProvider();
                var engines = services.GetServices<IBuilderEngine>().ToArray();
                if (engines is { Length: > 0 })
                {
                    foreach (var engine in engines)
                    {
                        engine.Run();
                    }
                }

                container.Populate(builder.Services);
            });
    }

    private static void StartAppEngines(this WebApplication app)
    {
        var engines = app.Services.GetServices<IAppEngine>().ToArray();
        if (engines is { Length: > 0 })
        {
            foreach (var engine in engines)
            {
                engine.Run();
            }
        }
    }
}