using CollectionSystem.Api.Infrastructure.Bases;

namespace CollectionSystem.Api.Infrastructure.SeqLog;

public class SeqSetting : IJsonFileSetting
{
    public string ServerUrl { get; set; }
    public string ApiKey { get; set; }
    public string JsonFilePath => "./SeqLog/seq-setting.json";
}