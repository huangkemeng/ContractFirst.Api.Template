using System.Linq.Expressions;
using ContractFirst.Api.Primary.Entities.Bases;

namespace ContractFirst.Api.Primary.Entities.EntityEvents.Bases;

public class EntityEventBuilder<T> where T : class, IEntityPrimary
{
    private readonly EntityEventsContainer container;
    private bool hasBuild;

    public EntityEventBuilder(EntityEventsContainer container)
    {
        EventEntityProperties = new List<EventEntityProperty<T>>();
        EventEnities = new List<EventEnity<T>>();
        this.container = container;
    }

    public List<EventEntityProperty<T>> EventEntityProperties { get; set; }
    public List<EventEnity<T>> EventEnities { get; set; }

    public EventEntityProperty<T> Property<T1>(Expression<Func<T, T1>> property)
    {
        if (!hasBuild)
        {
            var entityProperty = new EventEntityProperty<T>();
            entityProperty = entityProperty.Property(property);
            EventEntityProperties.Add(entityProperty);
            return entityProperty;
        }

        throw new InvalidOperationException("This builder has already built");
    }

    public EventEnity<T> Entity()
    {
        if (!hasBuild)
        {
            var eventEnity = new EventEnity<T>();
            EventEnities.Add(eventEnity);
            return eventEnity;
        }

        throw new InvalidOperationException("This builder has already built");
    }

    public void Build()
    {
        if (hasBuild) throw new InvalidOperationException("This builder has already built");
        var entityType = typeof(T);
        foreach (var eventEntityProperty in EventEntityProperties)
        foreach (var propertyEvent in eventEntityProperty.Events)
        foreach (var propertyEventItem in propertyEvent.Value)
            container.AddPropertyEvent(entityType, propertyEvent.Key, propertyEventItem);

        foreach (var eventEnity in EventEnities)
        foreach (var entityEvent in eventEnity.Events)
            container.AddEntityEvent(entityType, entityEvent);
        hasBuild = true;
    }
}