using CollectionSystem.Api.Infrastructure.Bases;

namespace CollectionSystem.Api.Infrastructure.EfCore;

public class DbSetting : IJsonFileSetting
{
    public string ConnectionString { get; set; }
    public string JsonFilePath => "./EfCore/db-setting.json";
}