using MassTransit;
using MediatR;
using Order.Domain.Aggregates;
using Order.Domain.Repositories;
using Shared.Exceptions.Exceptions;

namespace Order.Application.Commands
{
    public record FulfillOrderCommand(Guid? BuyerId, Guid OrderId) : IRequest;

    public class FulfillOrderCommandHandler(
        IOrderRepository orders,
        ILicenseKeyRepository licenseKeys,
        IPublishEndpoint publishEndpoint) : IRequestHandler<FulfillOrderCommand>
    {
        public async Task Handle(FulfillOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await MarkPaidCommandHandler.LoadOwnedOrderAsync(
                orders, request.BuyerId, request.OrderId, cancellationToken);

            var keys = await licenseKeys.GetByOrderIdAsync(order.Id, cancellationToken);
            if (!ReservationMatches(order, keys))
                throw new ConflictException("Reserved keys do not match the order.");

            foreach (var key in keys)
            {
                key.Assign(order.Id);
                await licenseKeys.UpdateAsync(key, cancellationToken);
            }

            order.Fulfill();
            await orders.UpdateAsync(order, cancellationToken);
            await OrderEventPublisher.PublishAsync(order, publishEndpoint, cancellationToken);
            order.ClearDomainEvents();
            await orders.SaveChangesAsync(cancellationToken);
        }

        private static bool ReservationMatches(Domain.Aggregates.Order order, IReadOnlyList<LicenseKey> keys)
        {
            var expected = order.OrderLinesReadOnly.Sum(line => line.Quantity);
            if (keys.Count != expected)
                return false;

            foreach (var line in order.OrderLinesReadOnly)
            {
                var lineKeys = keys.Where(key => key.GameId == line.GameId).ToList();
                if (lineKeys.Count != line.Quantity)
                    return false;

                if (lineKeys.Any(key => key.LicenseKeyStatus != LicenseKeyStatus.Reserved))
                    return false;
            }

            return true;
        }
    }
}
