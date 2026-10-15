using Order.Domain.Aggregates;

namespace Order.Domain.DomainEvent
{
    public record OrderPlaced(
        Guid BuyerId,
        Guid CheckoutId,
        DateTime CreatedAt,
        IReadOnlyCollection<OrderLineRestore> Lines,
        DateTime OccurredOn
        ) : IDomainEvent;
}
