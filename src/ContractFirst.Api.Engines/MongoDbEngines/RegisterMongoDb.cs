using System.Reflection;
using Autofac;
using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.Infrastructure.MongoDb;
using ContractFirst.Api.Primary.Entities.Bases;
using MongoDB.Driver;

namespace ContractFirst.Api.Engines.MongoDbEngines;

public class RegisterMongoDb : IBuilderEngine
{
    private readonly ContainerBuilder containerBuilder;

    public RegisterMongoDb(ContainerBuilder containerBuilder)
    {
        this.containerBuilder = containerBuilder;
    }

    public void Run()
    {
        containerBuilder
            .RegisterType<MongoDbContext>()
            .AsSelf()
            .SingleInstance();
        var mongoEntityBaseType = typeof(IMongoDbEntity);
        var mongoEntityTypes = typeof(IEntityPrimary).Assembly
            .ExportedTypes
            .Where(x => x is { IsClass: true, IsAbstract: false } && mongoEntityBaseType.IsAssignableFrom(x))
            .ToArray();
        var dbType = typeof(IMongoDatabase);
        var getCollectionMethodType = dbType.GetMethod(nameof(IMongoDatabase.GetCollection),
            BindingFlags.Public | BindingFlags.Instance)!;
        foreach (var mongoEntityType in mongoEntityTypes)
            containerBuilder.Register(e =>
                {
                    var context = e.Resolve<MongoDbContext>();
                    var getCollectionGenericType = getCollectionMethodType.MakeGenericMethod(mongoEntityType);
                    return getCollectionGenericType.Invoke(context.Database,
                        new object?[] { mongoEntityType.Name, null });
                })
                .As(typeof(IMongoCollection<>).MakeGenericType(mongoEntityType))
                .InstancePerLifetimeScope();
    }
}