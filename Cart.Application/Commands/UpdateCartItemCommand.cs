using Cart.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Cart.Application.Commands
{
    public record UpdateCartItemCommand(Guid? BuyerId, Guid GameId, int Quantity) : IRequest<long>;

    public class UpdateCartItemCommandHandler(ICartRepository cartRepository)
        : IRequestHandler<UpdateCartItemCommand, long>
    {
        public async Task<long> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            if (request.BuyerId == null || request.BuyerId == Guid.Empty)
                throw new BadRequestException("User don't have an ID");

            var cart = await cartRepository.GetByBuyerIdAsync(request.BuyerId.Value, cancellationToken);
            if (cart == null)
                throw new NotFoundException("Cart was not found.");

            if (!cart.CartItemsReadOnly.Any(item => item.GameId == request.GameId))
                throw new NotFoundException("Game was not found in the cart.");

            cart.ChangeQuantity(request.GameId, request.Quantity);
            await cartRepository.UpdateAsync(cart, cancellationToken);
            await cartRepository.SaveChangesAsync(cancellationToken);

            return cart.TotalPrice;
        }
    }
}
