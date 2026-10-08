using Identity.Domain.Aggregates.IdentityAggregate;

namespace Identity.Domain.Security
{
    public interface ITokenService
    {
        string GenerateAccessToken(Account account);
        string GenerateRefreshToken();
    }
}
