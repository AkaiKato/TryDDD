using Catalog.Domain.Helpers;
using Catalog.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Catalog.Application.Commands
{
    public record PublishGameCommand(Guid GameId) : IRequest;

    public class PublishGameCommandHandler(
        IGameRepository gameRepository,
        IOutbox outbox
        ) : IRequestHandler<PublishGameCommand>
    {
        public async Task Handle(PublishGameCommand request, CancellationToken cancellationToken)
        {
            var existingGame = await gameRepository.GetByIdAsync(request.GameId, cancellationToken)
                ?? throw new NotFoundException($"Not found game with id {request.GameId}");
            existingGame.Publish();
            await gameRepository.UpdateAsync(existingGame, cancellationToken);
            await outbox.EnqueueAsync(existingGame.DomainEvents, cancellationToken);
            existingGame.ClearDomainEvents();
            await gameRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
