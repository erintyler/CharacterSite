using CharacterSite.Domain.Common;
using CharacterSite.Domain.Repositories;
using Wolverine;

namespace CharacterSite.Infrastructure.Repositories;

public class UnitOfWork(CharacterDbContext context, IMessageBus bus) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var events = aggregateRoot.GetDomainEvents();
                aggregateRoot.ClearDomainEvents();
                return events;
            });
        
        await context.SaveChangesAsync(cancellationToken);
        
        foreach (var domainEvent in domainEvents)
        {
            await bus.PublishAsync(domainEvent);
        }
    }
}