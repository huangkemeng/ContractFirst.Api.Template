using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ContractFirst.Api.Primary.Entities.EntityEvents.Bases;

public interface IEntityEvent
{
    Task Handle(EntityEntry entry, CancellationToken cancellationToken);
}