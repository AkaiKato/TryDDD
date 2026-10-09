namespace Catalog.Domain.DomainEvent
{
    public record GameArchived(Guid GameId, DateTime OccurredOn) : IDomainEvent;
}
