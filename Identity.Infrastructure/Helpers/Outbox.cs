using Identity.Domain.DomainEvent;
using Identity.Domain.Heplers;
using Identity.Infrastructure.Data;
using System.Text.Json;

namespace Identity.Infrastructure.Helpers
{
    public class Outbox(IdentityDbContext context) : IOutbox
    {
        public async Task EnqueueAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                context.OutboxMessages.Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().FullName,
                    Json = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    OccurredOn = domainEvent.OccurredOn
                });
            }
        }
    }
}
