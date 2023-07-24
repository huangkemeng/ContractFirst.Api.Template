using CollectionSystem.Api.Primary.Entities.Bases;

namespace CollectionSystem.Api.Primary.Entities.EntityEvents.Bases;

public class EventEnity<T> where T : class, IEntityPrimary
{
    public EventEnity()
    {
        Events = new HashSet<EventWrapper>(new EntityWrapperEqualityComparer());
    }

    public HashSet<EventWrapper> Events { get; }

    public EventEnity<T> HasAddedEvent<TEvent>()
        where TEvent : IEntityEvent
    {
        return HasAddedEvent(typeof(TEvent));
    }

    public EventEnity<T> HasUpdatedEvent<TEvent>()
        where TEvent : IEntityEvent
    {
        return HasUpdatedEvent(typeof(TEvent));
    }

    public EventEnity<T> HasDeletedEvent<TEvent>()
        where TEvent : IEntityEvent
    {
        return HasDeletedEvent(typeof(TEvent));
    }

    public EventEnity<T> HasAddedEvent(Type type)
    {
        CheckType(type);
        Events.Add(new EventWrapper
        {
            EventType = type,
            EventTiming = EventTimingEnum.EntityAdded
        });
        return this;
    }

    public EventEnity<T> HasUpdatedEvent(Type type)
    {
        CheckType(type);
        Events.Add(new EventWrapper
        {
            EventType = type,
            EventTiming = EventTimingEnum.EntityUpdated
        });
        return this;
    }

    public EventEnity<T> HasDeletedEvent(Type type)
    {
        CheckType(type);
        Events.Add(new EventWrapper
        {
            EventType = type,
            EventTiming = EventTimingEnum.EntityDeleted
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