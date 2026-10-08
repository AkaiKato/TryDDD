using Identity.Domain.DomainEvent;

namespace Identity.Domain.Heplers
{
    public interface IOutbox
    {
        public Task EnqueueAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
