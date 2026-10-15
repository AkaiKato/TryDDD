using Cart.Domain.DomainEvent;
using Cart.Domain.Repositories;
using MassTransit;
using MediatR;
using Shared.Exceptions.Exceptions;

namespace Cart.Application.Commands
{
    public record RequestCheckoutCommand(Guid? BuyerId) : IRequest<Guid>;

    public class RequestCheckoutCommandHandler(
        ICartRepository cartRepository,
        IPublishEndpoint publishEndpoint
        ) : IRequestHandler<RequestCheckoutCommand, Guid>
    {
        public async Task<Guid> Handle(RequestCheckoutCommand request, CancellationToken cancellationToken)
        {
            if (request.BuyerId == null || request.BuyerId == Guid.Empty)
                throw new BadRequestException("User id is not valid");

            var cart = await cartRepository.GetByBuyerIdAsync(request.BuyerId.Value, cancellationToken);
            if (cart == null || cart.CartItemsReadOnly.Count == 0)
                throw new BadRequestException("Cart is empty");
            var requestId = Guid.NewGuid();
            cart.RequestCheckout(requestId);

            await cartRepository.UpdateAsync(cart, cancellationToken);
            await publishEndpoint.Publish<CheckoutRequested>(cart.DomainEvents.OfType<CheckoutRequested>().Single(), cancellationToken);
            cart.ClearDomainEvents();
            await cartRepository.SaveChangesAsync(cancellationToken);

            return requestId;
        }
    }
}
