using CollectionSystem.Api.Infrastructure.Bases;

namespace CollectionSystem.Api.Infrastructure.EfCore;

public class DbSetting : IJsonFileSetting
{
    public DbConnectionStringSetting ConnectionStrings { get; set; }
    public string JsonFilePath => "./EfCore/db-setting.json";
}

public class DbConnectionStringSetting
{
    public string WebApi { get; set; }
    public string IntegrationTest { get; set; }
}