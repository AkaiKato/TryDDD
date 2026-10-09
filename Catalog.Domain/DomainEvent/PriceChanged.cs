namespace Catalog.Domain.DomainEvent
{
    public record PriceChanged(Guid GameId, long NewPrice, DateTime OccurredOn) : IDomainEvent;
}
