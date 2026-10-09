namespace Catalog.Domain.DomainEvent
{
    public record GamePublished(Guid GameId, string Name, long Price, DateTime OccurredOn) : IDomainEvent;
}
