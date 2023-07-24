using System.Linq.Expressions;
using System.Reflection;
using CollectionSystem.Api.Primary.Entities.Bases;

namespace CollectionSystem.Api.Primary.Entities.EntityEvents.Bases;

public class EventEntityProperty<T> where T : class, IEntityPrimary
{
    public EventEntityProperty()
    {
        Events = new Dictionary<PropertyInfo, HashSet<EventWrapper>>();
    }

    public Dictionary<PropertyInfo, HashSet<EventWrapper>> Events { get; }

    public EventEntityProperty<T> Property<TProperty>(Expression<Func<T, TProperty>> property)
    {
        if (property.Body is MemberExpression member)
            if (member.Member is PropertyInfo propertyInfo)
                if (!Events.ContainsKey(propertyInfo))
                    Events.Add(propertyInfo, new HashSet<EventWrapper>(new EntityWrapperEqualityComparer()));
        return this;
    }

    public EventEntityProperty<T> HasUpdatedEvent<TEvent>() where TEvent : IEntityEvent
    {
        return HasUpdatedEvent(typeof(TEvent));
    }

    public EventEntityProperty<T> HasUpdatedEvent(Type type)
    {
        CheckType(type);
        foreach (var property in Events)
            property.Value.Add(new EventWrapper
            {
                EventType = type,
                EventTiming = EventTimingEnum.EntityPropertyUpdated
            });
        return this;
    }

    private void CheckType(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (!typeof(IEntityEvent).IsAssignableFrom(type))
            throw new InvalidOperationException("The provided type must implement IEntityEvent.");
    }
}