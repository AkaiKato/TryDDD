namespace Order.Domain.DomainEvent
{
    public record OrderFulfilled(
        Guid BuyerId,
        Guid CheckoutId,
        DateTime OccurredOn
        ) : IDomainEvent;
}
