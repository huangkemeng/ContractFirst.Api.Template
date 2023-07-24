using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.Infrastructure.Bases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContractFirst.Api.Engines.SettingEngines;

public class RegisterSettings : IBuilderEngine
{
    private readonly IServiceCollection services;

    public RegisterSettings(IServiceCollection services)
    {
        this.services = services;
    }

    public void Run()
    {
        var iSettingType = typeof(ISetting);
        var infrastructureAssembly = iSettingType.Assembly;
        var settingTypes = infrastructureAssembly
            ?.ExportedTypes
            .Where(e => e.GetInterfaces().Contains(iSettingType) && e.IsClass && !e.IsAbstract)
            .ToArray();
        var basePath = AppContext.BaseDirectory;
        IConfigurationBuilder appConfigurationBuilder = new ConfigurationBuilder();
        if (settingTypes != null)
            foreach (var settingType in settingTypes)
            {
                var setting = Activator.CreateInstance(settingType)!;
                var settingName = settingType.Name;
                if (setting is IJsonFileSetting jsonFileSetting)
                {
                    var path = Path.Combine(basePath, jsonFileSetting.JsonFilePath);
                    if (!string.IsNullOrWhiteSpace(jsonFileSetting.JsonFilePath)
                        && File.Exists(path))
                        appConfigurationBuilder.AddJsonFile(path, false);
                }
                else if (setting is IStringSetting stringSetting)
                {
                    services.AddSingleton(settingType, stringSetting);
                }
            }

        if (File.Exists(Path.Combine(basePath, "appsettings.json")))
            appConfigurationBuilder
                .AddJsonFile(Path.Combine(basePath, "appsettings.json"), false);
        ;
        var configuration = appConfigurationBuilder
            .AddEnvironmentVariables()
            .Build();
        services.AddSingleton(configuration);
        services.AddSingleton<IConfiguration>(configuration);
        if (settingTypes != null)
            foreach (var settingType in settingTypes)
            {
                var setting = Activator.CreateInstance(settingType)!;
                var settingName = settingType.Name;
                configuration.GetSection(settingName).Bind(setting);
                services.AddSingleton(settingType, setting);
            }
    }
}