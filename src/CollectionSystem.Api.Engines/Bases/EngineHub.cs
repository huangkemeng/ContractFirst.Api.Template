using Autofac;
using Autofac.Extensions.DependencyInjection;
using CollectionSystem.Api.Infrastructure.Bases;
using CollectionSystem.Api.Infrastructure.SeqLog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CollectionSystem.Api.Engines.Bases;

public static class EngineHub
{
    public static WebApplication BuildWithEngines(this WebApplicationBuilder builder)
    {
        var configuration = builder.Services.AddSettings(e => e.Scene = SceneOptions.WebApi);
        builder.Configuration.AddConfiguration(configuration);
        builder.Logging.AddSeqLog();
        builder.Services.RegisterEngines();
        builder.StartBuilderEngines();
        WebApplication? builtApp = default;
        builder.Services.AddSingleton(_ => builtApp ??= builder.Build());
        var serviceProvider = builder.Services.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<WebApplication>();
        app.Services.StartAppEngines();
        return app;
    }

    private static void RegisterEngines(this IServiceCollection services)
    {
        var iEngineType = typeof(IEngine);
        var engineTypes = iEngineType
            .Assembly
            .ExportedTypes
            .Where(e => e.GetInterfaces().Contains(iEngineType) && e is { IsClass: true, IsAbstract: false })
            .ToList();

        foreach (var engineType in engineTypes)
        {
            var iType = engineType.GetInterfaces()
                .FirstOrDefault(e => e.GetInterfaces().Contains(iEngineType) && e != iEngineType);
            if (iType != null) services.AddSingleton(iType, engineType);
        }
    }

    private static void StartBuilderEngines(this WebApplicationBuilder builder)
    {
        builder.Host.UseServiceProviderFactory(new CustomAutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((_, container) =>
            {
                builder.Services.StartBuilderEngines(container);
            });
    }

    private static void StartBuilderEngines(this IServiceCollection serviceCollection,
        ContainerBuilder containerBuilder)
    {
        serviceCollection.AddSingleton(serviceCollection);
        serviceCollection.AddSingleton(containerBuilder);
        var services = serviceCollection.BuildServiceProvider();
        var engines = services.GetServices<IBuilderEngine>().ToArray();
        if (engines is { Length: > 0 })
            foreach (var engine in engines)
                engine.Run();

        containerBuilder.Populate(serviceCollection);
    }

    private static void StartAppEngines(this IServiceProvider services)
    {
        var engines = services.GetServices<IAppEngine>().ToArray();
        if (engines is { Length: > 0 })
            foreach (var engine in engines)
                engine.Run();
    }

    public static IContainer TestBuildWithEngines(this ContainerBuilder containerBuilder,
        Action<ContainerBuilder>? builderAction = null)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        var configuration = serviceCollection.AddSettings(e => e.Scene = SceneOptions.Test);
        configurationBuilder.AddConfiguration(configuration);
        var configurationRoot = configurationBuilder.Build();
        serviceCollection.AddSingleton(configurationRoot);
        serviceCollection.AddSingleton<IConfiguration>(configurationRoot);
        serviceCollection.RegisterEngines();
        serviceCollection.StartBuilderEngines(containerBuilder);
        serviceCollection.BuildServiceProvider();
        builderAction?.Invoke(containerBuilder);
        return containerBuilder.Build();
    }
}