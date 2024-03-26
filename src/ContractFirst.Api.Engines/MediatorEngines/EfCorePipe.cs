using Autofac;
using ContractFirst.Api.Infrastructure.EfCore;
using ContractFirst.Api.Primary.Entities.EntityEvents.Bases;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ContractFirst.Api.Engines.MediatorEngines;

public class EfCorePipe : IPipeSpecification<IReceiveContext<IMessage>>
{
    private readonly ApplicationDbContext? dbContext;
    private readonly ILifetimeScope lifetimeScope;

    public EfCorePipe(ILifetimeScope lifetimeScope)
    {
        this.lifetimeScope = lifetimeScope;
        dbContext = lifetimeScope.Resolve<ApplicationDbContext>();
    }

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
        if (dbContext is { ChangeTracker: not null })
        {
            //有DML操作才需要SaveChangesAsync
            var hasDml = dbContext.ChangeTracker.Entries().Any(e =>
                new[] { EntityState.Deleted, EntityState.Modified, EntityState.Added }.Contains(e.State));
            if (hasDml)
            {
                await HandleEntityEvents(cancellationToken, dbContext).ConfigureAwait(false);
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public Task OnException(Exception ex, IReceiveContext<IMessage> context)
    {
        throw ex;
    }

    private async Task HandleEntityEvents(CancellationToken cancellationToken, DbContext context)
    {
        var entries = context.ChangeTracker.Entries().Where(e =>
            new[] { EntityState.Deleted, EntityState.Modified, EntityState.Added }.Contains(e.State));
        var container = lifetimeScope.Resolve<EntityEventsContainer>();
        var eventDict = new Dictionary<EntityEntry, HashSet<IEntityEvent>>();
        foreach (var entry in entries)
        {
            var type = entry.Entity.GetType();
            if (container.EntityEvents.TryGetValue(type, out var entityEventWrappers))
                foreach (var entityEventWrapper in entityEventWrappers)
                    if ((entry.State == EntityState.Added &&
                         entityEventWrapper.EventTiming == EventTimingEnum.EntityAdded)
                        || (entry.State == EntityState.Modified &&
                            entityEventWrapper.EventTiming == EventTimingEnum.EntityUpdated)
                        || (entry.State == EntityState.Deleted &&
                            entityEventWrapper.EventTiming == EventTimingEnum.EntityDeleted))
                        if (lifetimeScope.TryResolve(entityEventWrapper.EventType, out var eventInterface)
                            && eventInterface is IEntityEvent entityEvent)
                        {
                            if (!eventDict.ContainsKey(entry)) eventDict.Add(entry, new HashSet<IEntityEvent>());

                            if (eventInterface is ISingleRunEntityEvent singleRunEntityEvent)
                            {
                                var existedEvents = eventDict.SelectMany(e => e.Value).ToList();
                                if (!existedEvents.Contains(singleRunEntityEvent))
                                    eventDict[entry].Add(singleRunEntityEvent);
                            }
                            else if (eventInterface is IMultipleRunEntityEvent multipleRunEntityEvent)
                            {
                                eventDict[entry].Add(multipleRunEntityEvent);
                            }
                            else
                            {
                                eventDict[entry].Add(entityEvent);
                            }
                        }

            if (container.PropertyEvents.ContainsKey(type))
                foreach (var property in entry.Properties)
                {
                    var propertyInfo = property.Metadata.PropertyInfo;
                    if (property.IsModified
                        && propertyInfo != null
                        && container.PropertyEvents[type].Keys.Any(e =>
                            e.PropertyType == propertyInfo.PropertyType && e.Name == propertyInfo.Name))
                    {
                        var propertyEventWrappers = container.PropertyEvents[type].First(e =>
                                e.Key.PropertyType == propertyInfo.PropertyType && e.Key.Name == propertyInfo.Name)
                            .Value;
                        foreach (var propertyEventWrapper in propertyEventWrappers)
                            if (propertyEventWrapper.EventTiming == EventTimingEnum.EntityPropertyUpdated
                                && lifetimeScope.TryResolve(propertyEventWrapper.EventType,
                                    out var propertyEventInterface)
                                && propertyEventInterface is IEntityEvent entityEvent)
                            {
                                if (!eventDict.ContainsKey(entry)) eventDict.Add(entry, new HashSet<IEntityEvent>());

                                if (propertyEventInterface is ISingleRunEntityEvent singleRunEntityEvent)
                                {
                                    var existedEvents = eventDict.SelectMany(e => e.Value).ToList();
                                    if (!existedEvents.Contains(singleRunEntityEvent))
                                        eventDict[entry].Add(singleRunEntityEvent);
                                }
                                else if (propertyEventInterface is IMultipleRunEntityEvent multipleRunEntityEvent)
                                {
                                    eventDict[entry].Add(multipleRunEntityEvent);
                                }
                                else
                                {
                                    eventDict[entry].Add(entityEvent);
                                }
                            }
                    }
                }
        }

        foreach (var eventItem in eventDict)
        foreach (var item in eventItem.Value)
            await item.Handle(eventItem.Key, cancellationToken);
    }
}