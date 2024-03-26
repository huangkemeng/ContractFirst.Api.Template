﻿using ContractFirst.Api.Infrastructure.Bases;

namespace ContractFirst.Api.Infrastructure.DataPersistence.EfCore;

public class DbSetting : IJsonFileSetting
{
    public DbConnectionStringSetting ConnectionStrings { get; set; }
    public string JsonFilePath => "./DataPersistence/EfCore/db-setting.json";
}

public class DbConnectionStringSetting
{
    public string WebApi { get; set; }
    public string IntegrationTest { get; set; }
}