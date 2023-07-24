using ContractFirst.Api.Primary.Bases;
using ContractFirst.Api.Primary.Entities.Bases;
using ContractFirst.Api.Primary.Entities.EntityEvents;
using ContractFirst.Api.Realization.Bases;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ContractFirst.Api.Realization.EntityEvents;

[AsType(LifetimeEnum.Scope, typeof(IMainEntityAddedEvent))]
public class MainEntityAddedEvent : IMainEntityAddedEvent
{
    //Maybe you need a  kind of ICurrent<User> object to get current userId
    public MainEntityAddedEvent()
    {
    }

    public Task Handle(EntityEntry entry, CancellationToken cancellationToken)
    {
        if (entry.Entity is IMainEntity mainEntity)
        {
            // todo
            // mainEntity.CreatedBy = userId;
        }

        return Task.CompletedTask;
    }
}