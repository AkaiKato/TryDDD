using Identity.Domain.Aggregates.IdentityAggregate;

namespace Identity.Infrastructure.Data
{
    public class AccountRow
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public AccountStatus Status { get; set; }
    }
}
