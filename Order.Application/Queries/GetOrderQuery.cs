using MediatR;
using Order.Domain.Aggregates;
using Order.Domain.Repositories;

namespace Order.Application.Queries
{
    public record OrderLineResponse(Guid GameId, string Title, long UnitPrice, int Quantity, long Total);

    public record OrderKeyResponse(Guid GameId, string Code);

    public record OrderResponse(
        Guid OrderId,
        Guid CheckoutId,
        string Status,
        long TotalPrice,
        bool NeedRefund,
        DateTime CreatedAt,
        IReadOnlyList<OrderLineResponse> Lines,
        IReadOnlyList<OrderKeyResponse> Keys);

    public record GetOrderQuery(Guid? BuyerId, Guid OrderId) : IRequest<OrderResponse>;

    public class GetOrderQueryHandler(
        IOrderRepository orders,
        ILicenseKeyRepository licenseKeys) : IRequestHandler<GetOrderQuery, OrderResponse>
    {
        public async Task<OrderResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await Commands.MarkPaidCommandHandler.LoadOwnedOrderAsync(
                orders, request.BuyerId, request.OrderId, cancellationToken);

            var lines = order.OrderLinesReadOnly
                .Select(line => new OrderLineResponse(
                    line.GameId,
                    line.Title,
                    line.Price.Amount,
                    line.Quantity,
                    line.TotalForLine))
                .ToList();

            IReadOnlyList<OrderKeyResponse> keys = [];
            if (order.OrderStatus == OrderStatus.Fulfilled)
            {
                var stored = await licenseKeys.GetByOrderIdAsync(order.Id, cancellationToken);
                keys = stored
                    .Select(key => new OrderKeyResponse(key.GameId, key.Code))
                    .ToList();
            }

            return new OrderResponse(
                order.Id,
                order.CheckoutId,
                order.OrderStatus.ToString(),
                order.TotalPrice,
                order.NeedRefund,
                order.CreatedAt,
                lines,
                keys);
        }
    }
}
