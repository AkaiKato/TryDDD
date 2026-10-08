using Identity.Domain.Building_blocks;

namespace Identity.Domain.Aggregates.IdentityAggregate
{
    public class RefreshSession : AggregateRoot<Guid>
    {
        public Guid AccountId { get; private set; }

        public string TokenHash { get; private set; }
        
        public DateTime Expiration { get; private set; }

        public DateTime? RevokedAt { get; private set; }
    
        private RefreshSession(Guid id, Guid accountId, string tokenHash, DateTime expiration) : base(id)
        {
            AccountId = accountId;
            TokenHash = tokenHash;
            Expiration = expiration;
        }

        public static RefreshSession Create(Guid accountId, string tokenHash, DateTime expiration)
        {
            var refreshSession = new RefreshSession(Guid.NewGuid(), accountId, tokenHash, expiration);
            return refreshSession;
        }

        public static RefreshSession Restore(Guid id, Guid accountId, string tokenHash, DateTime expiration, DateTime? revokedAt)
        {
            var refreshSession = new RefreshSession(id, accountId, tokenHash, expiration)
            {
                RevokedAt = revokedAt
            };
            return refreshSession;
        }

        public bool IsActive() => RevokedAt is null && Expiration > DateTime.UtcNow;

        public void Rotate(string newTokenHash, DateTime newExpiration)
        {
            if (RevokedAt != null)
                throw new InvalidOperationException("Refresh session is revoked.");

            if (string.IsNullOrWhiteSpace(newTokenHash))
                throw new ArgumentException("Token hash cannot be empty.", nameof(newTokenHash));

            if (newExpiration <= DateTime.UtcNow)
                throw new ArgumentException("Expiration must be in the future.", nameof(newExpiration));

            TokenHash = newTokenHash;
            Expiration = newExpiration;
        }

        public void Revoke()
        {
            if (RevokedAt != null)
                return;

            RevokedAt = DateTime.UtcNow;
        }
    }
}
