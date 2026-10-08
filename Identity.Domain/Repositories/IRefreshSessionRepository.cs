using Identity.Domain.Aggregates.IdentityAggregate;

namespace Identity.Domain.Repositories
{
    public interface IRefreshSessionRepository
    {
        Task<RefreshSession?> GetRefreshSessionByTokenHash(string tokenHash, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshSession refreshSession, CancellationToken cancellationToken = default);

        Task UpdateAsync(RefreshSession refreshSession, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
