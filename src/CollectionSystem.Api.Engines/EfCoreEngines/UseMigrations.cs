using Autofac;
using CollectionSystem.Api.Engines.Bases;
using CollectionSystem.Api.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace CollectionSystem.Api.Engines.EfCoreEngines;

public class UseMigrations : IAppEngine
{
    private readonly ILifetimeScope migrateScope;

    public UseMigrations(ILifetimeScope migrateScope)
    {
        this.migrateScope = migrateScope;
    }

    public void Run()
    {
        var dbContext = migrateScope.Resolve<SqlDbContext>();
        var connectString = dbContext.Database.GetConnectionString();
        if (!string.IsNullOrWhiteSpace(connectString))
        {
            var migrations = dbContext.Database.GetMigrations();
            if (migrations.Any() && dbContext.Database.CanConnect())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}