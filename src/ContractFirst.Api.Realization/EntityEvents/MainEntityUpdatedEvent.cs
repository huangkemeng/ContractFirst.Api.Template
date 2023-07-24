using ContractFirst.Api.Primary.Entities.Bases;
using ContractFirst.Api.Primary.Entities.EntityEvents;
using ContractFirst.Api.Realization.Bases;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ContractFirst.Api.Realization.EntityEvents;

[AsType(LifetimeEnum.Scope, typeof(IMainEntityUpdatedEvent))]
public class MainEntityUpdatedEvent:IMainEntityUpdatedEvent
{
    public Task Handle(EntityEntry entry, CancellationToken cancellationToken)
    {
        if (entry.Entity is IMainEntity mainEntity)
        {
            // todo 
            //mainEntity.UpdatedBy = staffId;
            //mainEntity.UpdatedBy = DateTimeOffset.Now;
        }
        return Task.CompletedTask;
    }
}