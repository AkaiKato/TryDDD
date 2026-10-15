using Microsoft.EntityFrameworkCore;
using Order.Domain.Aggregates;
using Order.Domain.Repositories;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class LicenseKeyRepository(OrderDbContext context) : ILicenseKeyRepository
    {
        public async Task AddAsync(LicenseKey licenseKey, CancellationToken cancellationToken = default)
        {
            await context.LicenseKeyRows.AddAsync(ToRow(licenseKey), cancellationToken);
        }

        public Task UpdateAsync(LicenseKey licenseKey, CancellationToken cancellationToken = default)
        {
            var row = context.LicenseKeyRows.Local.FirstOrDefault(x => x.Id == licenseKey.Id);
            if (row == null)
                throw new InvalidOperationException($"License key {licenseKey.Id} is not loaded.");

            row.LicenseKeyStatus = licenseKey.LicenseKeyStatus;
            row.OrderId = licenseKey.OrderId;
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<LicenseKey>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("OrderId can't be empty", nameof(orderId));

            var rows = await context.LicenseKeyRows
                .Where(x => x.OrderId == orderId)
                .ToListAsync(cancellationToken);

            return rows.Select(ToLicenseKey).ToList();
        }

        public async Task<IReadOnlyList<LicenseKey>> GetAvailableByGameIdAsync(
            Guid gameId,
            int count,
            CancellationToken cancellationToken = default)
        {
            if (gameId == Guid.Empty)
                throw new ArgumentException("GameId can't be empty", nameof(gameId));

            if (count < 1)
                throw new ArgumentOutOfRangeException(nameof(count));

            var rows = await context.LicenseKeyRows
                .Where(x => x.GameId == gameId && x.LicenseKeyStatus == LicenseKeyStatus.Available)
                .OrderBy(x => x.Id)
                .Take(count)
                .ToListAsync(cancellationToken);

            return rows.Select(ToLicenseKey).ToList();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        private static LicenseKeyRow ToRow(LicenseKey licenseKey) => new()
        {
            Id = licenseKey.Id,
            GameId = licenseKey.GameId,
            Code = licenseKey.Code,
            LicenseKeyStatus = licenseKey.LicenseKeyStatus,
            OrderId = licenseKey.OrderId
        };

        private static LicenseKey ToLicenseKey(LicenseKeyRow row)
            => LicenseKey.Restore(row.Id, row.GameId, row.Code, row.LicenseKeyStatus, row.OrderId);
    }
}
