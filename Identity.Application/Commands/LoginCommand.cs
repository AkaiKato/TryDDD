using Identity.Domain.Aggregates.IdentityAggregate;
using Identity.Domain.DTOs;
using Identity.Domain.Heplers;
using Identity.Domain.Repositories;
using Identity.Domain.Security;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Identity.Application.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<LoginDto>;

    public class LoginCommandHandler(
        IAccountRepository accountRepository,
        IRefreshSessionRepository refreshSessionRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
        : IRequestHandler<LoginCommand, LoginDto>
    {
        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var account = await accountRepository.GetByEmailAsync(request.Email, cancellationToken)
                ?? throw new NotFoundException($"Not found user with email {request.Email}");

            if (!account.CanSignIn())
                throw new ForbiddenException($"Account blocked");

            var passwordEquals = passwordHasher.VerifyHashedPassword(request.Password, account.PasswordHash);
            if (!passwordEquals)
                throw new BadRequestException($"Wrong password");

            var accessToken = tokenService.GenerateAccessToken(account);
            var refreshToken = tokenService.GenerateRefreshToken();
            var session = RefreshSession.Create(account.Id, refreshToken.TokenHash, refreshToken.ExpiresAt);

            await refreshSessionRepository.AddAsync(session, cancellationToken);
            await refreshSessionRepository.SaveChangesAsync(cancellationToken);

            return new LoginDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
