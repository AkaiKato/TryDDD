using Catalog.Domain.DTO;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Queries
{
    public record GetAllGamesQuery() : IRequest<IEnumerable<GameDTO>>;
    
    public class GetAllGamesQueryHandler(
        IGameRepository gameRepository)
        : IRequestHandler<GetAllGamesQuery, IEnumerable<GameDTO>>
    {
        public async Task<IEnumerable<GameDTO>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
        {
            var games = await gameRepository.GetAllGamesAsync(cancellationToken);
            return games;
        }
    }
}
