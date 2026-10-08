using Identity.Domain.Aggregates.IdentityAggregate;

namespace Identity.Domain.Security
{
    public record IssuedRefreshToken(string Token, string TokenHash, DateTime ExpiresAt);

    public interface ITokenService
    {
        string GenerateAccessToken(Account account);
        IssuedRefreshToken GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);
    }
}
