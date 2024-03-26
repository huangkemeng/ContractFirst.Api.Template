using Autofac;
using ContractFirst.Api.Infrastructure.DataPersistence.EfCore;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;

namespace ContractFirst.Api.Engines.MediatorEngines;

public class EfCorePipe(ILifetimeScope lifetimeScope) : IPipeSpecification<IReceiveContext<IMessage>>
{
    private readonly ApplicationDbContext? _dbContext = lifetimeScope.Resolve<ApplicationDbContext>();

    public bool ShouldExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
    {
        return true;
    }

    public Task BeforeExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
    {
        return Task.WhenAll();
    }

    public Task Execute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
    {
        return Task.WhenAll();
    }

    public async Task AfterExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
    {
        if (_dbContext is { ChangeTracker: not null })
        {
            if (_dbContext.ChangeTracker.HasChanges())
            {
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public Task OnException(Exception ex, IReceiveContext<IMessage> context)
    {
        throw ex;
    }
}