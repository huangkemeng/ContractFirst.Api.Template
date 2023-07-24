using Autofac;
using ContractFirst.Api.Engines.Bases;
using Microsoft.EntityFrameworkCore;

namespace ContractFirst.Api.Engines.EfCoreEngines;

public class UseMigrations : IAppEngine
{
    private readonly ILifetimeScope migrateScope;

    public UseMigrations(ILifetimeScope migrateScope)
    {
        this.migrateScope = migrateScope;
    }

    public void Run()
    {
        var dbContext = migrateScope.Resolve<DbContext>()!;
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