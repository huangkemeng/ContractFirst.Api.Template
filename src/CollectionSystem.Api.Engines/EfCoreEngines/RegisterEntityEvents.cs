using System.Reflection;
using Autofac;
using CollectionSystem.Api.Engines.Bases;
using CollectionSystem.Api.Primary.Entities.Bases;
using CollectionSystem.Api.Primary.Entities.EntityEvents;
using CollectionSystem.Api.Primary.Entities.EntityEvents.Bases;

namespace CollectionSystem.Api.Engines.EfCoreEngines;

public class RegisterEntityEvents : IBuilderEngine
{
    private readonly ContainerBuilder containerBuilder;

    public RegisterEntityEvents(ContainerBuilder containerBuilder)
    {
        this.containerBuilder = containerBuilder;
    }

    public void Run()
    {
        var type = typeof(IHasEntityEvent<>);
        var iMainType = typeof(IMainEntity);
        var eventAssembly = type.Assembly;
        var eventTypes = eventAssembly
            .ExportedTypes
            .Where(e => e.IsClass && !e.IsAbstract &&
                        (e.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == type) ||
                         iMainType.IsAssignableFrom(e)))
            .ToList();
        var eventBuilderType = typeof(EntityEventBuilder<>);
        var addEventType = typeof(IMainEntityAddedEvent);
        var changedEventType = typeof(IMainEntityUpdatedEvent);
        var eventEntityType = typeof(EventEnity<>);
        var entityEventsContainer = new EntityEventsContainer();
        foreach (var eventType in eventTypes)
        {
            Type? parameterType = null;
            object? parameterObject = null;
            if (iMainType.IsAssignableFrom(eventType))
            {
                parameterType = eventBuilderType.MakeGenericType(eventType);
                var eventEntityGenericType = eventEntityType.MakeGenericType(eventType);
                parameterObject = Activator.CreateInstance(parameterType, entityEventsContainer);
                var entityMethod = parameterType.GetMethod(nameof(EntityEventBuilder<IEntityPrimary>.Entity),
                    BindingFlags.Public | BindingFlags.Instance);
                if (entityMethod != null)
                {
                    var entity = entityMethod.Invoke(parameterObject, null);
                    var hasAddedEventMethod = eventEntityGenericType
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(e =>
                            e.Name == nameof(EventEnity<IEntityPrimary>.HasAddedEvent) && e.IsGenericMethod);
                    hasAddedEventMethod = hasAddedEventMethod?.MakeGenericMethod(addEventType);
                    entity = hasAddedEventMethod?.Invoke(entity, null);
                    var hasChangedEventMethod = eventEntityGenericType
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(e =>
                            e.Name == nameof(EventEnity<IEntityPrimary>.HasUpdatedEvent) && e.IsGenericMethod);
                    hasChangedEventMethod = hasChangedEventMethod?.MakeGenericMethod(changedEventType);
                    entity = hasChangedEventMethod?.Invoke(entity, null);
                }
            }

            var configurationMethod = eventType.GetMethod(nameof(IHasEntityEvent<IEntityPrimary>.ConfigureEntityEvent),
                BindingFlags.Static | BindingFlags.Public);
            if (configurationMethod != null)
            {
                var configurationMethodParameter = configurationMethod.GetParameters().First();
                if (parameterType == null) parameterType = configurationMethodParameter.ParameterType;
                if (parameterObject == null)
                    parameterObject = Activator.CreateInstance(parameterType, entityEventsContainer);
                configurationMethod.Invoke(null, new[] { parameterObject });
            }

            var buildMethod = parameterType?.GetMethod(nameof(EntityEventBuilder<IEntityPrimary>.Build),
                BindingFlags.Public | BindingFlags.Instance);
            buildMethod?.Invoke(parameterObject, null);
            containerBuilder.RegisterInstance(parameterObject!)
                .As(parameterType!)
                .SingleInstance();
        }

        containerBuilder.RegisterInstance(entityEventsContainer!)
            .AsSelf()
            .SingleInstance();
    }
}