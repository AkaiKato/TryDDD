using Identity.Domain.Aggregates.IdentityAggregate;

namespace Identity.Domain.Repositories
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account, CancellationToken cancellationToken = default);

        Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task UpdateAsync(Account account, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
