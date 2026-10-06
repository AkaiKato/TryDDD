namespace Identity.Domain.DomainEvent
{
    public record AccountRegistered(Guid AccountId, DateTime OccurredOn) : IDomainEvent;
}
