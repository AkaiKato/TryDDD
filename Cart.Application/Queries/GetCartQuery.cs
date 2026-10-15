using Cart.Domain.Repositories;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Cart.Application.Queries
{
    public record CartItemResponse(Guid GameId, string Title, long Price, int Quantity);

    public record CartResponse(
        Guid CartId,
        Guid BuyerId,
        string Status,
        long TotalPrice,
        IReadOnlyList<CartItemResponse> Items);

    public record GetCartQuery(Guid? BuyerId) : IRequest<CartResponse>;

    public class GetCartQueryHandler(ICartRepository cartRepository) : IRequestHandler<GetCartQuery, CartResponse>
    {
        public async Task<CartResponse> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            if (request.BuyerId is null || request.BuyerId == Guid.Empty)
                throw new BadRequestException("User don't have an ID");

            var cart = await cartRepository.GetByBuyerIdAsync(request.BuyerId.Value, cancellationToken);
            if (cart is null)
                throw new NotFoundException("Cart was not found.");

            var items = cart.CartItemsReadOnly
                .Select(item => new CartItemResponse(item.GameId, item.Title, item.Price.Amount, item.Quantity))
                .ToList();

            return new CartResponse(cart.Id, cart.BuyerId, cart.CartStatus.ToString(), cart.TotalPrice, items);
        }
    }
}
