using Order.Domain.Aggregates;

namespace Order.Domain.Repositories
{
    public interface ILicenseKeyRepository
    {
        Task AddAsync(LicenseKey licenseKey, CancellationToken cancellationToken = default);
        Task UpdateAsync(LicenseKey licenseKey, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<LicenseKey>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<LicenseKey>> GetAvailableByGameIdAsync(Guid gameId, int count, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
