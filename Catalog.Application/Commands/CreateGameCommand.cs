using Catalog.Domain.Aggregates;
using Catalog.Domain.Helpers;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Commands
{
    public record CreateGameCommand(string Title, decimal Price) : IRequest<Guid>;

    public class CreateGameCommandHandler(
        IGameRepository gameRepository,
        IOutbox outbox
        ) : IRequestHandler<CreateGameCommand, Guid>
    {
        public async Task<Guid> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            var game = Game.Create(request.Title, request.Price);
            await gameRepository.AddAsync(game, cancellationToken);
            await outbox.EnqueueAsync(game.DomainEvents, cancellationToken);
            game.ClearDomainEvents();
            await gameRepository.SaveChangesAsync(cancellationToken);
            return game.Id;
        }
    }
}
