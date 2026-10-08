using Identity.Domain.Aggregates.IdentityAggregate;

namespace Identity.Domain.DTOs
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public AccountStatus AccountStatus { get; set; }
    }
}
