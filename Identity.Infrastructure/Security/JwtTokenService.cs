using Identity.Domain.Aggregates.IdentityAggregate;
using Identity.Domain.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Security
{
    public class JwtTokenService(IConfiguration config) : ITokenService
    {
        public string GenerateAccessToken(Account account)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(config["Jwt:AccessTokenLifetimeMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public IssuedRefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            RandomNumberGenerator.Fill(randomNumber);
            var token = Convert.ToBase64String(randomNumber);
            var lifetimeDays = config["Jwt:RefreshTokenLifetimeDays"]
                ?? throw new InvalidOperationException("Jwt:RefreshTokenLifetimeDays is missing");

            return new IssuedRefreshToken(
                token,
                HashRefreshToken(token),
                DateTime.UtcNow.AddDays(Convert.ToDouble(lifetimeDays)));
        }

        public string HashRefreshToken(string refreshToken)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
