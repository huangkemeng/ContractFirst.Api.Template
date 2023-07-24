using CollectionSystem.Api.Primary.Entities.Bases;

namespace CollectionSystem.Api.Primary.Entities.EntityEvents.Bases;

public interface IHasEntityEvent<T> where T : class, IEntityPrimary
{
    static abstract void ConfigureEntityEvent(EntityEventBuilder<T> builder);
}