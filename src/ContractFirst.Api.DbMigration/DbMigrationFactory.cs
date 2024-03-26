using Autofac;
using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.Infrastructure.Bases;
using ContractFirst.Api.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContractFirst.Api.DbMigration;

public class DbMigrationFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new ContainerBuilder();
        var container = builder.SceneBuildWithEngines(SceneOptions.WebApi);
        var sqlDbContext = container.Resolve<ApplicationDbContext>();
        return sqlDbContext;
    }
}