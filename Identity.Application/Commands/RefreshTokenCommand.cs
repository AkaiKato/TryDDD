using Identity.Domain.DTOs;
using Identity.Domain.Repositories;
using Identity.Domain.Security;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Identity.Application.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginDto>;

    public class RefreshTokenCommandHandler(
        IAccountRepository accountRepository,
        IRefreshSessionRepository refreshSessionRepository,
        ITokenService tokenService)
        : IRequestHandler<RefreshTokenCommand, LoginDto>
    {
        public async Task<LoginDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new BadRequestException("Refresh token is missing.");

            var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
            var session = await refreshSessionRepository.GetRefreshSessionByTokenHash(tokenHash, cancellationToken)
                ?? throw new UnauthorizedException("Refresh token is invalid.");

            if (!session.IsActive())
                throw new UnauthorizedException("Refresh token is invalid.");

            var account = await accountRepository.GetByIdAsync(session.AccountId, cancellationToken)
                ?? throw new UnauthorizedException("Refresh token is invalid.");

            if (!account.CanSignIn())
                throw new ForbiddenException("Account blocked");

            var accessToken = tokenService.GenerateAccessToken(account);
            var refreshToken = tokenService.GenerateRefreshToken();
            session.Rotate(refreshToken.TokenHash, refreshToken.ExpiresAt);

            await refreshSessionRepository.UpdateAsync(session, cancellationToken);
            await refreshSessionRepository.SaveChangesAsync(cancellationToken);

            return new LoginDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
