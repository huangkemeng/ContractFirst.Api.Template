using System.Reflection;

namespace CollectionSystem.Api.Primary.Entities.EntityEvents.Bases;

public class EntityEventsContainer
{
    public EntityEventsContainer()
    {
        PropertyEvents = new Dictionary<Type, Dictionary<PropertyInfo, HashSet<EventWrapper>>>();
        EntityEvents = new Dictionary<Type, HashSet<EventWrapper>>();
    }

    public Dictionary<Type, Dictionary<PropertyInfo, HashSet<EventWrapper>>> PropertyEvents { get; }

    public Dictionary<Type, HashSet<EventWrapper>> EntityEvents { get; }

    public void AddPropertyEvent(Type entityType, PropertyInfo propertyInfo, EventWrapper eventWrapper)
    {
        if (!PropertyEvents.ContainsKey(entityType))
            PropertyEvents.Add(entityType, new Dictionary<PropertyInfo, HashSet<EventWrapper>>());
        if (!PropertyEvents[entityType].ContainsKey(propertyInfo))
            PropertyEvents[entityType]
                .Add(propertyInfo, new HashSet<EventWrapper>(new EntityWrapperEqualityComparer()));
        PropertyEvents[entityType][propertyInfo].Add(eventWrapper);
    }

    public void AddEntityEvent(Type entityType, EventWrapper eventWrapper)
    {
        if (!EntityEvents.ContainsKey(entityType))
            EntityEvents.Add(entityType, new HashSet<EventWrapper>(new EntityWrapperEqualityComparer()));
        EntityEvents[entityType].Add(eventWrapper);
    }
}