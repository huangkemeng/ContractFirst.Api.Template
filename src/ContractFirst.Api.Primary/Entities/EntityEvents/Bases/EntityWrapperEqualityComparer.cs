using System.Diagnostics.CodeAnalysis;

namespace ContractFirst.Api.Primary.Entities.EntityEvents.Bases;

public class EntityWrapperEqualityComparer : IEqualityComparer<EventWrapper>
{
    public bool Equals(EventWrapper? x, EventWrapper? y)
    {
        if (x != null && y != null) return x.EventType == y.EventType && x.EventTiming == y.EventTiming;
        return x == y;
    }

    public int GetHashCode([DisallowNull] EventWrapper obj)
    {
        return obj.GetHashCode();
    }
}