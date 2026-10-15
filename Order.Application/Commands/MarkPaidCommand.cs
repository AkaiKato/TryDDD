using MassTransit;
using MediatR;
using Order.Domain.Repositories;
using Shared.Exceptions.Exceptions;
using OrderAggregate = Order.Domain.Aggregates.Order;

namespace Order.Application.Commands
{
    public record MarkPaidCommand(Guid? BuyerId, Guid OrderId) : IRequest;

    public class MarkPaidCommandHandler(
        IOrderRepository orders,
        IPublishEndpoint publishEndpoint) : IRequestHandler<MarkPaidCommand>
    {
        public async Task Handle(MarkPaidCommand request, CancellationToken cancellationToken)
        {
            var order = await LoadOwnedOrderAsync(orders, request.BuyerId, request.OrderId, cancellationToken);
            order.MarkPaid();
            await orders.UpdateAsync(order, cancellationToken);
            await OrderEventPublisher.PublishAsync(order, publishEndpoint, cancellationToken);
            order.ClearDomainEvents();
            await orders.SaveChangesAsync(cancellationToken);
        }

        internal static async Task<OrderAggregate> LoadOwnedOrderAsync(
            IOrderRepository orders,
            Guid? buyerId,
            Guid orderId,
            CancellationToken cancellationToken)
        {
            if (buyerId is null || buyerId == Guid.Empty)
                throw new BadRequestException("User id is not valid");

            if (orderId == Guid.Empty)
                throw new BadRequestException("Order id is not valid");

            var order = await orders.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
                throw new NotFoundException("Order was not found.");

            if (order.BuyerId != buyerId)
                throw new ForbiddenException("Order belongs to another buyer.");

            return order;
        }
    }
}
