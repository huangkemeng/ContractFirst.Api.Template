using Autofac;
using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.Infrastructure.EfCore;
using ContractFirst.Api.Primary.Entities.Bases;
using Microsoft.EntityFrameworkCore;

namespace ContractFirst.Api.Engines.EfCoreEngines;

public class RegisterDbSets : IBuilderEngine
{
    private readonly ContainerBuilder container;
    private readonly EngineBuilderOptions builderOptions;

    public RegisterDbSets(ContainerBuilder container, EngineBuilderOptions builderOptions)
    {
        this.container = container;
        this.builderOptions = builderOptions;
    }

    public void Run()
    {
        if (builderOptions.EnableEfCore)
        {
            AddDbContext();
        }
    }

    private void AddDbContext()
    {
        container.RegisterType<SqlDbContext>()
            .AsSelf()
            .As<DbContext>()
            .InstancePerLifetimeScope();
        var idbEntityType = typeof(IEfDbEntity<>);
        var idbEntityAssembly = idbEntityType.Assembly;
        var dbEntityTypes = idbEntityAssembly
            ?.ExportedTypes
            .Where(e => e.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == idbEntityType) &&
                        e.IsClass && !e.IsAbstract)
            .ToArray();
        if (dbEntityTypes != null && dbEntityTypes.Any())
        {
            var dbSetMethodType = typeof(SqlDbContext).GetMethods()
                .First(e => e.Name == nameof(DbContext.Set) && e.GetParameters().Length == 0);
            var dbSetType = typeof(DbSet<>);
            foreach (var dbEntityType in dbEntityTypes)
            {
                var dbSettMethodGenericType = dbSetMethodType.MakeGenericMethod(dbEntityType);
                var dbSetGenericType = dbSetType.MakeGenericType(dbEntityType);
                container.Register(c =>
                    {
                        var dbContext = c.Resolve<SqlDbContext>();
                        return dbSettMethodGenericType.Invoke(dbContext, null);
                    })
                    .As(dbSetGenericType);
            }
        }
    }
}