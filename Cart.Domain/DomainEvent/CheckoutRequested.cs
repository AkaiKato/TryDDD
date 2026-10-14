using Cart.Domain.Aggregates;

namespace Cart.Domain.DomainEvent
{
    public record CheckoutRequested(
        Guid CheckoutId,
        Guid BuyerId,
        IReadOnlyList<CartLine> Lines,
        DateTime OccurredOn) : IDomainEvent;
}
