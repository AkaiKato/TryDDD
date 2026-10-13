using Catalog.Domain.Aggregates;
using Catalog.Domain.DTO;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories
{
    public class GameRepository(CatalogDbContext context) : IGameRepository
    {
        public async Task<IEnumerable<GameDTO>> GetAllGamesAsync(CancellationToken cancellationToken = default)
        {
            var games = await context.Games.AsNoTracking()
                .Select(g => new GameDTO
                {
                    Id = g.Id,
                    Title = g.Title,
                    Price = g.Price,
                    GameStatus = g.GameStatus
                }).ToListAsync(cancellationToken);
            return games;
        }

        public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await context.Games.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken); 

            return row == null ? null : ToGame(row);
        }

        public async Task AddAsync(Game game, CancellationToken cancellationToken = default)
        {
            context.Games.Add(ToRow(game));
        }

        public async Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
        {
            var row = await context.Games.FirstOrDefaultAsync(x => x.Id == game.Id, cancellationToken);
            if (row == null)
            {
                throw new InvalidOperationException($"Game with ID {game.Id} not found.");

            }

            row.GameStatus = game.GameStatus;
            row.Price = game.Price.Amount;
            row.Title = game.Name.Title;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        private static GameRow ToRow(Game game) => new()
        {
            Id = game.Id,
            Title = game.Name.Title,
            Price = game.Price.Amount,
            GameStatus = game.GameStatus
        };

        private static Game ToGame(GameRow row) => Game.Restore(row.Id, row.Title, row.Price, row.GameStatus);

    }
}
