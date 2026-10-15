namespace Order.Domain.DomainEvent
{
    public interface IDomainEvent
    {
        public DateTime OccurredOn { get; }
    }
}
