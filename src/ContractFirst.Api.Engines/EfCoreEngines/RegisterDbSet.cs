using Autofac;
using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.Infrastructure.DataPersistence.EfCore;
using ContractFirst.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;

namespace ContractFirst.Api.Engines.EfCoreEngines;
public class RegisterDbSet : IBuilderEngine
{
    private readonly ContainerBuilder _container;

    public RegisterDbSet(ContainerBuilder container)
    {
        _container = container;
    }

    public void Run()
    {
        _container.RegisterType<ApplicationDbContext>()
            .AsSelf()
            .As<DbContext>()
            .InstancePerLifetimeScope();
        var idbEntityType = typeof(IEfEntity<>);
        var idbEntityAssembly = idbEntityType.Assembly;
        var dbEntityTypes = idbEntityAssembly
            ?.ExportedTypes
            .Where(e => e.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == idbEntityType) &&
                        e is { IsClass: true, IsAbstract: false })
            .ToArray();
        if (dbEntityTypes != null && dbEntityTypes.Any())
        {
            var dbSetMethodType = typeof(ApplicationDbContext).GetMethods()
                .First(e => e.Name == nameof(DbContext.Set) && e.GetParameters().Length == 0);
            var dbSetType = typeof(DbSet<>);
            foreach (var dbEntityType in dbEntityTypes)
            {
                var dbSettMethodGenericType = dbSetMethodType.MakeGenericMethod(dbEntityType);
                var dbSetGenericType = dbSetType.MakeGenericType(dbEntityType);
                _container.Register(c =>
                    {
                        var dbContext = c.Resolve<ApplicationDbContext>();
                        return dbSettMethodGenericType.Invoke(dbContext, null);
                    })
                    .As(dbSetGenericType);
            }
        }
    }
}