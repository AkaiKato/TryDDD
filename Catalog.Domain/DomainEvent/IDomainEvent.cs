namespace Catalog.Domain.DomainEvent
{
    public interface IDomainEvent
    {
        public DateTime OccurredOn { get; }
    }
}
