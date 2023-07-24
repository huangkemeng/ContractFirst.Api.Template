using System.Diagnostics;
using ContractFirst.Api.Infrastructure.EfCore.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace ContractFirst.Api.Infrastructure.EfCore;

public class SqlDbContext : DbContext
{
    private readonly DbSetting dbSetting;

    public SqlDbContext(DbSetting dbSetting)
    {
        this.dbSetting = dbSetting;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 全局限制查询的条数 (Top1000)
        optionsBuilder
            .ReplaceService<IQueryTranslationPostprocessorFactory, TopRowsQueryTranslationPostprocessorFactory>();
        optionsBuilder.UseSqlServer(dbSetting.ConnectionString, options => { options.CommandTimeout(6000); });
        if (Debugger.IsAttached) optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.LoadFromEntityConfigure();
    }
}