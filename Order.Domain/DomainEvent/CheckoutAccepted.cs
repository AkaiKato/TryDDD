namespace Order.Domain.DomainEvent
{
    public record CheckoutAccepted(
        Guid CheckoutId,
        Guid BuyerId,
        Guid OrderId,
        DateTime OccurredOn) : IDomainEvent;
}
