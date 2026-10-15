namespace Order.Domain.DomainEvent
{
    public record CheckoutRejected(
        Guid CheckoutId,
        Guid BuyerId,
        DateTime OccurredOn) : IDomainEvent;
}
