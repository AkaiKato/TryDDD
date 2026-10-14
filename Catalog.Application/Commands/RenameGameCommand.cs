using Catalog.Domain.Helpers;
using Catalog.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Catalog.Application.Commands
{
    public record RenameGameCommand(Guid GameId, string NewTitle) : IRequest;

    public class RenameGameCommandHandler(
        IGameRepository gameRepository,
        IOutbox outbox
        ) : IRequestHandler<RenameGameCommand>
    {
        public async Task Handle(RenameGameCommand request, CancellationToken cancellationToken)
        {
            var existingGame = await gameRepository.GetByIdAsync(request.GameId, cancellationToken)
                ?? throw new NotFoundException($"Not found game with id {request.GameId}");
            existingGame.UpdateName(request.NewTitle);
            await gameRepository.UpdateAsync(existingGame, cancellationToken);
            await outbox.EnqueueAsync(existingGame.DomainEvents, cancellationToken);
            existingGame.ClearDomainEvents();
            await gameRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
