namespace Catalog.Domain.DomainEvent
{
    public record GameRenamed(Guid GameId, string NewName, DateTime OccurredOn) : IDomainEvent;
}
