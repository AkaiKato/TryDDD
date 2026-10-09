using Catalog.Domain.DomainEvent;

namespace Catalog.Domain.Building_blocks
{
    public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
    {
        private readonly List<IDomainEvent> domainEvents = [];
        public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

        protected AggregateRoot(TId id) : base(id)
        {
        }

        protected void RaiseDomainEvent(IDomainEvent domainEvent) 
            => domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => domainEvents.Clear();
    }
}
