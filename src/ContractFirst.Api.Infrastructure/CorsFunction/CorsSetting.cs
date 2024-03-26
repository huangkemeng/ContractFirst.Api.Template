﻿using ContractFirst.Api.Infrastructure.Bases;

namespace ContractFirst.Api.Infrastructure.CorsFunction;

public class CorsSetting : IJsonFileSetting
{
    public string[] Origins { get; set; }
    public string JsonFilePath => "./CorsFunction/cors-setting.json";
}