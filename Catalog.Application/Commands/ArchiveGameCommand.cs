using Catalog.Domain.Helpers;
using Catalog.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Catalog.Application.Commands
{
    public record ArchiveGameCommand(Guid GameId) : IRequest;

    public class ArchiveGameCommandHandler(
        IGameRepository gameRepository,
        IOutbox outbox
        ) : IRequestHandler<ArchiveGameCommand>
    {
        public async Task Handle(ArchiveGameCommand request, CancellationToken cancellationToken)
        {
            var existingGame = await gameRepository.GetByIdAsync(request.GameId, cancellationToken)
                ?? throw new NotFoundException($"Not found game with id {request.GameId}");
            existingGame.Archive();
            await gameRepository.UpdateAsync(existingGame, cancellationToken);
            await outbox.EnqueueAsync(existingGame.DomainEvents, cancellationToken);
            existingGame.ClearDomainEvents();
            await gameRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
