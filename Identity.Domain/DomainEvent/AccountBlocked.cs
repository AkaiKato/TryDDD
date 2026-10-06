namespace Identity.Domain.DomainEvent
{
    public record AccountBlocked(Guid AccountId, DateTime OccurredOn) : IDomainEvent; 
}
