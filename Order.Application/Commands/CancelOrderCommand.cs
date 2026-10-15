using MassTransit;
using MediatR;
using Order.Domain.Aggregates;
using Order.Domain.Repositories;

namespace Order.Application.Commands
{
    public record CancelOrderCommand(Guid? BuyerId, Guid OrderId) : IRequest;

    public class CancelOrderCommandHandler(
        IOrderRepository orders,
        ILicenseKeyRepository licenseKeys,
        IPublishEndpoint publishEndpoint) : IRequestHandler<CancelOrderCommand>
    {
        public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await MarkPaidCommandHandler.LoadOwnedOrderAsync(
                orders, request.BuyerId, request.OrderId, cancellationToken);

            if (order.OrderStatus is not (OrderStatus.Placed or OrderStatus.Paid))
                throw new InvalidOperationException("Only a placed or paid order can be cancelled");

            var keys = await licenseKeys.GetByOrderIdAsync(order.Id, cancellationToken);
            foreach (var key in keys)
            {
                key.Release(order.Id);
                await licenseKeys.UpdateAsync(key, cancellationToken);
            }

            order.Cancel();
            await orders.UpdateAsync(order, cancellationToken);
            await OrderEventPublisher.PublishAsync(order, publishEndpoint, cancellationToken);
            order.ClearDomainEvents();
            await orders.SaveChangesAsync(cancellationToken);
        }
    }
}
