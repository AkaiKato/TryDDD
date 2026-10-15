using Microsoft.EntityFrameworkCore;
using Order.Domain.Aggregates;
using Order.Domain.Repositories;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrderRepository
    {
        public async Task<Domain.Aggregates.Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await context.Orders.Include(x => x.OrderLines).AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

            return order == null ? null : ToOrder(order);
        }

        public async Task<Domain.Aggregates.Order?> GetByCheckoutIdAsync(Guid checkoutId, CancellationToken cancellationToken = default)
        {
            var order = await context.Orders.Include(x => x.OrderLines).AsNoTracking()
                .FirstOrDefaultAsync(x => x.CheckoutId == checkoutId, cancellationToken);

            return order == null ? null : ToOrder(order);
        }

        public async Task AddAsync(Domain.Aggregates.Order order, CancellationToken cancellationToken = default)
        {
            await context.Orders.AddAsync(ToRow(order));
        }

        public async Task UpdateAsync(Domain.Aggregates.Order order, CancellationToken cancellationToken = default)
        {
            var row = await context.Orders.Include(x => x.OrderLines)
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellationToken);

            if (row == null)
                throw new InvalidOperationException($"Order with ID {order.Id} not found.");

            row.OrderStatus = order.OrderStatus;
            row.NeedRefund = order.NeedRefund;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await OrderDbSave.SaveAsync(context, cancellationToken);
        }

        private static OrderRow ToRow(Order.Domain.Aggregates.Order order)
        {
            List<OrderLineRow> orderLines = [];

            foreach (var item in order.OrderLinesReadOnly)
            {
                orderLines.Add(ToOrderLineRow(order.Id, item));
            }

            return new OrderRow
            {
                Id = order.Id,
                OrderStatus = order.OrderStatus,
                BuyerId =order.BuyerId,
                CheckoutId = order.CheckoutId,
                CreatedAt = order.CreatedAt,
                NeedRefund = order.NeedRefund,
                OrderLines = orderLines
            };
        }

        private static Domain.Aggregates.Order ToOrder(OrderRow row)
        {
            List<OrderLineRestore> orderLines = [];

            foreach (var item in row.OrderLines)
            {
                orderLines.Add(
                    new OrderLineRestore
                    {
                        GameId = item.GameId,
                        UnitPrice = item.Price,
                        Quantity = item.Quantity,
                        Title = item.Title
                    }
                    );
            }

            return Domain.Aggregates.Order.Restore(row.Id, row.BuyerId, row.CheckoutId, row.OrderStatus, orderLines, row.NeedRefund, row.CreatedAt);
        }

        private static OrderLineRow ToOrderLineRow(Guid orderId, OrderLine item) => new()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            GameId = item.GameId,
            Price = item.Price.Amount,
            Quantity = item.Quantity,
            Title = item.Title
        };
    }
}
