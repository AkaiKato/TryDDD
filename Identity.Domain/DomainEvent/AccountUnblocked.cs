namespace Identity.Domain.DomainEvent
{
    public record AccountUnblocked(Guid AccountId, DateTime OccurredOn) : IDomainEvent;
}
