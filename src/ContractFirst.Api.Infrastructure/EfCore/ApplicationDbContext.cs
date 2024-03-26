using System.Diagnostics;
using ContractFirst.Api.Infrastructure.Bases;
using ContractFirst.Api.Infrastructure.EfCore.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace ContractFirst.Api.Infrastructure.EfCore;

public class ApplicationDbContext : DbContext
{
    private readonly DbSetting dbSetting;
    private readonly SettingOptions settingOptions;

    public ApplicationDbContext(DbSetting dbSetting, SettingOptions settingOptions)
    {
        this.dbSetting = dbSetting;
        this.settingOptions = settingOptions;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            settingOptions.Scene == SceneOptions.Test
                ? dbSetting.ConnectionStrings.IntegrationTest
                : dbSetting.ConnectionStrings.WebApi, options =>
            {
                options.CommandTimeout(6000);
                options.EnableRetryOnFailure(2);
            });
        if (Debugger.IsAttached) optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.LoadFromEntityConfigure();
    }
}