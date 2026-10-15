namespace Order.Domain.DomainEvent
{
    public record OrderPaid(
        Guid BuyerId,
        Guid CheckoutId,
        DateTime OccurredOn) : IDomainEvent;
}
