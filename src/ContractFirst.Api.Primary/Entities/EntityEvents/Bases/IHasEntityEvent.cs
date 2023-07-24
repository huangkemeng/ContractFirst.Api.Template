using ContractFirst.Api.Primary.Entities.Bases;

namespace ContractFirst.Api.Primary.Entities.EntityEvents.Bases;

public interface IHasEntityEvent<T> where T : class, IEntityPrimary
{
    static abstract void ConfigureEntityEvent(EntityEventBuilder<T> builder);
}