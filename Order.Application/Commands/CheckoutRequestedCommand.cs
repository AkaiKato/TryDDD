using MassTransit;
using MediatR;
using Order.Application.Checkouts;
using Order.Application.Snapshots;
using Order.Domain.Aggregates;
using Order.Domain.DomainEvent;
using Order.Domain.Repositories;
using Shared.Exceptions.Exceptions;
using OrderAggregate = Order.Domain.Aggregates.Order;

namespace Order.Application.Commands
{
    public readonly record struct CheckoutLine(Guid GameId, string Title, long UnitPrice, int Quantity);

    public record CheckoutRequestedCommand(
        Guid CheckoutId,
        Guid BuyerId,
        IReadOnlyList<CheckoutLine> Lines) : IRequest;

    public class CheckoutRequestedCommandHandler(
        IOrderRepository orders,
        ILicenseKeyRepository licenseKeys,
        IRejectedCheckoutRepository rejectedCheckouts,
        IGameSnapshotRepository snapshots,
        IPublishEndpoint publishEndpoint) : IRequestHandler<CheckoutRequestedCommand>
    {
        public async Task Handle(CheckoutRequestedCommand request, CancellationToken cancellationToken)
        {
            if (request.CheckoutId == Guid.Empty || request.BuyerId == Guid.Empty)
                throw new BadRequestException("CheckoutId and BuyerId are required");

            var existing = await orders.GetByCheckoutIdAsync(request.CheckoutId, cancellationToken);
            if (existing != null)
            {
                await AcceptAsync(existing, cancellationToken);
                await orders.SaveChangesAsync(cancellationToken);
                return;
            }

            if (await rejectedCheckouts.ExistsAsync(request.CheckoutId, cancellationToken))
            {
                await RejectAsync(request.CheckoutId, request.BuyerId, cancellationToken);
                return;
            }

            if (!await LinesMatchProjectionAsync(request.Lines, cancellationToken))
            {
                await RejectAsync(request.CheckoutId, request.BuyerId, cancellationToken);
                return;
            }

            var batches = new List<IReadOnlyList<LicenseKey>>(request.Lines.Count);
            foreach (var line in request.Lines)
            {
                var available = await licenseKeys.GetAvailableByGameIdAsync(line.GameId, line.Quantity, cancellationToken);
                if (available.Count < line.Quantity)
                {
                    await RejectAsync(request.CheckoutId, request.BuyerId, cancellationToken);
                    return;
                }

                batches.Add(available);
            }

            var orderLines = request.Lines
                .Select(line => new OrderLineRestore(line.GameId, line.Title, line.UnitPrice, line.Quantity))
                .ToList();

            OrderAggregate order;
            try
            {
                order = OrderAggregate.Place(request.BuyerId, request.CheckoutId, orderLines);
            }
            catch (InvalidOperationException)
            {
                await RejectAsync(request.CheckoutId, request.BuyerId, cancellationToken);
                return;
            }

            for (var index = 0; index < batches.Count; index++)
            {
                foreach (var key in batches[index])
                {
                    key.Reserve(order.Id);
                    await licenseKeys.UpdateAsync(key, cancellationToken);
                }
            }

            await orders.AddAsync(order, cancellationToken);
            await AcceptAsync(order, cancellationToken);
            await OrderEventPublisher.PublishAsync(order, publishEndpoint, cancellationToken);
            order.ClearDomainEvents();

            try
            {
                await orders.SaveChangesAsync(cancellationToken);
            }
            catch (ConflictException)
            {
                existing = await orders.GetByCheckoutIdAsync(request.CheckoutId, cancellationToken);
                if (existing != null)
                {
                    await AcceptAsync(existing, cancellationToken);
                    await orders.SaveChangesAsync(cancellationToken);
                    return;
                }

                await RejectAsync(request.CheckoutId, request.BuyerId, cancellationToken);
            }
        }

        private async Task<bool> LinesMatchProjectionAsync(
            IReadOnlyList<CheckoutLine> lines,
            CancellationToken cancellationToken)
        {
            if (lines == null || lines.Count == 0)
                return false;

            if (lines.Select(line => line.GameId).Distinct().Count() != lines.Count)
                return false;

            foreach (var line in lines)
            {
                if (line.GameId == Guid.Empty || string.IsNullOrWhiteSpace(line.Title))
                    return false;

                if (line.Quantity < 1 || line.Quantity > 10 || line.UnitPrice <= 0)
                    return false;

                var game = await snapshots.GetGameByIdAsync(line.GameId, cancellationToken);
                if (game == null || !game.ForSale || game.Price != line.UnitPrice)
                    return false;
            }

            return true;
        }

        private async Task AcceptAsync(OrderAggregate order, CancellationToken cancellationToken)
        {
            await publishEndpoint.Publish(
                new CheckoutAccepted(order.CheckoutId, order.BuyerId, order.Id, DateTime.UtcNow),
                cancellationToken);
        }

        private async Task RejectAsync(Guid checkoutId, Guid buyerId, CancellationToken cancellationToken)
        {
            if (!await rejectedCheckouts.ExistsAsync(checkoutId, cancellationToken))
                await rejectedCheckouts.AddAsync(checkoutId, cancellationToken);

            await publishEndpoint.Publish(
                new CheckoutRejected(checkoutId, buyerId, DateTime.UtcNow),
                cancellationToken);
            await orders.SaveChangesAsync(cancellationToken);
        }
    }
}
