using Catalog.Domain.DomainEvent;

namespace Catalog.Domain.Helpers
{
    public interface IOutbox
    {
        public Task EnqueueAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
