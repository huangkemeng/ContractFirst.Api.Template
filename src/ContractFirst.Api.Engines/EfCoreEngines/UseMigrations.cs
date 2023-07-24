using Autofac;
using ContractFirst.Api.Engines.Bases;
using Microsoft.EntityFrameworkCore;

namespace ContractFirst.Api.Engines.EfCoreEngines;

public class UseMigrations : IAppEngine
{
    private readonly ILifetimeScope migrateScope;
    private readonly EngineBuilderOptions builderOptions;

    public UseMigrations(ILifetimeScope migrateScope, EngineBuilderOptions builderOptions)
    {
        this.migrateScope = migrateScope;
        this.builderOptions = builderOptions;
    }

    public void Run()
    {
        if (builderOptions.EnableEfCore)
        {
            RunEfCoreMigrations();
        }
    }

    private void RunEfCoreMigrations()
    {
        var newScope = migrateScope.BeginLifetimeScope();
        var dbContext = newScope.Resolve<DbContext>()!;
        var connectString = dbContext.Database.GetConnectionString();
        if (!string.IsNullOrWhiteSpace(connectString))
        {
            var migrations = dbContext.Database.GetMigrations();
            if (migrations.Any())
                if (dbContext.Database.CanConnect())
                    dbContext.Database.Migrate();
        }
    }
}