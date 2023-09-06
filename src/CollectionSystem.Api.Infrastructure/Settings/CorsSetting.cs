using CollectionSystem.Api.Infrastructure.Bases;

namespace CollectionSystem.Api.Infrastructure.Settings;

public class CorsSetting : IJsonFileSetting
{
    public string[] Origins { get; set; }
    public string JsonFilePath => "./Settings/cors-setting.json";
}