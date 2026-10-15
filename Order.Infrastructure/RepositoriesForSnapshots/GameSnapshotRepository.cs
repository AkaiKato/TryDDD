using Microsoft.EntityFrameworkCore;
using Order.Application.Snapshots;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.RepositoriesForSnapshots
{
    public class GameSnapshotRepository(OrderDbContext context) : IGameSnapshotRepository
    {
        public async Task<GameSnapshotData?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken)
        {
            var row = await context.GameSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GameId == gameId, cancellationToken);

            return row == null ? null : ToData(row);
        }

        public async Task ApplyPublishedAsync(Guid gameId, string title, long price, CancellationToken cancellationToken)
        {
            var row = await FindTrackedAsync(gameId, cancellationToken);
            if (row == null)
            {
                context.GameSnapshots.Add(new GameSnapshot
                {
                    GameId = gameId,
                    Title = title,
                    Price = price,
                    ForSale = true
                });
                return;
            }

            row.Title = title;
            row.Price = price;
            row.ForSale = true;
        }

        public async Task ApplyPriceChangedAsync(Guid gameId, long price, CancellationToken cancellationToken)
        {
            var row = await FindTrackedAsync(gameId, cancellationToken);
            if (row == null)
                return;

            row.Price = price;
        }

        public async Task ApplyRenamedAsync(Guid gameId, string title, CancellationToken cancellationToken)
        {
            var row = await FindTrackedAsync(gameId, cancellationToken);
            if (row == null)
                return;

            row.Title = title;
        }

        public async Task ApplyArchivedAsync(Guid gameId, CancellationToken cancellationToken)
        {
            var row = await FindTrackedAsync(gameId, cancellationToken);
            if (row == null)
                return;

            row.ForSale = false;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        private Task<GameSnapshot?> FindTrackedAsync(Guid gameId, CancellationToken cancellationToken)
            => context.GameSnapshots.FirstOrDefaultAsync(x => x.GameId == gameId, cancellationToken);

        private static GameSnapshotData ToData(GameSnapshot row)
            => new(row.GameId, row.Title, row.Price, row.ForSale);
    }
}
