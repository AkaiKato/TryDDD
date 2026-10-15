namespace Order.Domain.DomainEvent
{
    public record OrderCancelled(
        Guid BuyerId,
        Guid CheckoutId,
        bool NeedRefund,
        DateTime OccurredOn
        ) : IDomainEvent;
}
