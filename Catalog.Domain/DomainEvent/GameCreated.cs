namespace Catalog.Domain.DomainEvent
{
    public record GameCreated(Guid GameId, string Name, long Price, DateTime OccurredOn) : IDomainEvent;
}
