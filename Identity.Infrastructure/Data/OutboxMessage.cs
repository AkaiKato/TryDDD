namespace Identity.Infrastructure.Data
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string Json { get; set; } = null!;
        public DateTime OccurredOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
    }
}
