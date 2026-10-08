namespace Identity.Infrastructure.Data
{
    public class RefreshSessionRow
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string TokenHash { get; set; } = null!;

        public DateTime Expiration { get; set; }

        public DateTime? RevokedAt { get; set; }
    }
}
