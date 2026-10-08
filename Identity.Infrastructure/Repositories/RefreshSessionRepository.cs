using Identity.Domain.Aggregates.IdentityAggregate;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions.Exceptions;

namespace Identity.Infrastructure.Repositories
{
    public class RefreshSessionRepository(IdentityDbContext context) : IRefreshSessionRepository
    {
        public async Task AddAsync(RefreshSession refreshSession, CancellationToken cancellationToken = default)
        {
            context.RefreshSessions.Add(ToRow(refreshSession));
        }

        public async Task UpdateAsync(RefreshSession refreshSession, CancellationToken cancellationToken = default)
        {
            var row = await context.RefreshSessions
                .FirstOrDefaultAsync(x => x.Id == refreshSession.Id, cancellationToken);

            if (row is null)
            {
                throw new NotFoundException($"Refresh session {refreshSession.Id} was not found.");
            }

            row.TokenHash = refreshSession.TokenHash;
            row.Expiration = refreshSession.Expiration;
            row.RevokedAt = refreshSession.RevokedAt;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        private static RefreshSessionRow ToRow(RefreshSession refreshSession) => new()
        {
            Id = refreshSession.Id,
            AccountId = refreshSession.AccountId,
            TokenHash = refreshSession.TokenHash,
            Expiration = refreshSession.Expiration,
            RevokedAt = refreshSession.RevokedAt
        };

        private static RefreshSession ToRefreshSession(RefreshSessionRow row) =>
            RefreshSession.Restore(row.Id, row.AccountId, row.TokenHash, row.Expiration, row.RevokedAt);

        public async Task<RefreshSession?> GetRefreshSessionByTokenHash(string tokenHash, CancellationToken cancellationToken = default)
        {
            var row = await context.RefreshSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

            return row is null ? null : ToRefreshSession(row);
        }
    }
}
