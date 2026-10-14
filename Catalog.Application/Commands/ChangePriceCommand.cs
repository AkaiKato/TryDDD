using Catalog.Domain.Helpers;
using Catalog.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Catalog.Application.Commands
{
    public record ChangePriceCommand(Guid GameId, decimal NewPrice) : IRequest;

    public class ChangePriceCommandHandler(
        IGameRepository gameRepository,
        IOutbox outbox
        ) : IRequestHandler<ChangePriceCommand>
    {
        public async Task Handle(ChangePriceCommand request, CancellationToken cancellationToken)
        {
            var existingGame = await gameRepository.GetByIdAsync(request.GameId, cancellationToken)
                ?? throw new NotFoundException($"Not found game with id {request.GameId}");
            existingGame.UpdatePrice(request.NewPrice);
            await gameRepository.UpdateAsync(existingGame, cancellationToken);
            await outbox.EnqueueAsync(existingGame.DomainEvents, cancellationToken);
            existingGame.ClearDomainEvents();
            await gameRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
