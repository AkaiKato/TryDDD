using Catalog.Domain.Aggregates;
using Catalog.Domain.DTO;

namespace Catalog.Domain.Repositories
{
    public interface IGameRepository
    {
        Task AddAsync(Game game, CancellationToken cancellationToken = default);
        Task UpdateAsync(Game game, CancellationToken cancellationToken = default);
        Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<GameDTO>> GetAllGamesAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
