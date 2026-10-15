using Cart.Application.Snapshots;
using Cart.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Cart.Application.Commands
{
    public record AddToCartCommand(Guid? BuyerId, Guid GameId, int Quantity) : IRequest;

    public class AddToCartCommandHandler(
        ICartRepository cartRepository,
        IGameSnapshotRepository gameSnapshots
        ) : IRequestHandler<AddToCartCommand>
    {
        public async Task Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            if (request.BuyerId == null || request.BuyerId == Guid.Empty)
                throw new BadRequestException("User don't have an ID");

            var game = await gameSnapshots.GetGameByIdAsync(request.GameId, cancellationToken);
            if (game == null)
                throw new NotFoundException("Game was not found.");

            if (!game.ForSale)
                throw new BadRequestException("Game is not for sale.");

            var existingCart = await cartRepository.GetByBuyerIdAsync(request.BuyerId.Value, cancellationToken);
            var isNew = existingCart == null;
            existingCart ??= Domain.Aggregates.Cart.Create(request.BuyerId.Value);

            existingCart.AddItem(game.GameId, game.Title, game.Price, request.Quantity);

            if (isNew)
                await cartRepository.AddAsync(existingCart, cancellationToken);
            else
                await cartRepository.UpdateAsync(existingCart, cancellationToken);

            await cartRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
