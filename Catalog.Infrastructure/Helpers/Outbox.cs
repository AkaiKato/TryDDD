using Catalog.Domain.DomainEvent;
using Catalog.Domain.Helpers;
using Catalog.Infrastructure.Data;

namespace Catalog.Infrastructure.Helpers
{
    public class Outbox(CatalogDbContext context) : IOutbox
    {
        public async Task EnqueueAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                context.OutboxMessages.Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().FullName,
                    Json = System.Text.Json.JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    OccurredOn = domainEvent.OccurredOn
                });
            }
        }
    }
}
