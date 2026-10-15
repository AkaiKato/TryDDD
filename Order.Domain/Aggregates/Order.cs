using Order.Domain.Building_blocks;
using Order.Domain.DomainEvent;

namespace Order.Domain.Aggregates
{
    public class Order : AggregateRoot<Guid>
    {
        private List<OrderLine> OrderLines { get; set; } = [];

        public Guid BuyerId { get; private set; }
        public Guid CheckoutId { get; private set; }
        public OrderStatus OrderStatus { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool NeedRefund { get; private set; }
        public IReadOnlyCollection<OrderLine> OrderLinesReadOnly => OrderLines.AsReadOnly();
        public long TotalPrice => OrderLines.Sum(item => item.TotalForLine);

        private Order(Guid id, Guid buyerId, Guid checkoutId, 
            OrderStatus orderStatus, List<OrderLine> orderLines, bool needRefund,
            DateTime createAt) : base(id)
        {
            BuyerId = buyerId;
            CheckoutId = checkoutId;
            OrderStatus = orderStatus;
            OrderLines = orderLines?.ToList() ?? [];
            NeedRefund = needRefund;
            CreatedAt = createAt;
        }

        public static Order Place(Guid buyerId, Guid checkoutId, IReadOnlyList<OrderLineRestore> lines)
        {
            if(buyerId == Guid.Empty || checkoutId == Guid.Empty)
            {
                throw new InvalidOperationException("buyerId OR checkoutId is empty");
            }

            if (lines == null || lines.Count == 0)
                throw new ArgumentException(nameof(lines));

            if (lines.Select(x => x.GameId).Distinct().Count() != lines.Count)
                throw new InvalidOperationException("Order can't contain two lines for the same game");

            var items = lines
                .Select(x => OrderLine.Restore(x.GameId, x.Title, x.UnitPrice, x.Quantity))
                .ToList();

            var order = new Order(Guid.NewGuid(), buyerId, checkoutId, OrderStatus.Placed, items, false, DateTime.UtcNow);
            order.RaiseDomainEvent(new OrderPlaced(order.BuyerId, order.CheckoutId, order.CreatedAt, lines, DateTime.UtcNow));
            return order;
        }

        public static Order Restore(
            Guid id,
            Guid buyerId,
            Guid checkoutId,
            OrderStatus orderStatus,
            IReadOnlyList<OrderLineRestore> lines,
            bool needRefund,
            DateTime createdAt)
        {
            if (buyerId == Guid.Empty || checkoutId == Guid.Empty)
            {
                throw new InvalidOperationException("buyerId OR checkoutId is empty");
            }

            if (lines == null || lines.Count == 0)
                throw new ArgumentException(nameof(lines));

            if (lines.Select(x => x.GameId).Distinct().Count() != lines.Count)
                throw new InvalidOperationException("Order can't contain two lines for the same game");

            var items = lines
                .Select(x => OrderLine.Restore(x.GameId, x.Title, x.UnitPrice, x.Quantity))
                .ToList();

            return new Order(id, buyerId, checkoutId, orderStatus, items, needRefund, createdAt);
        }

        public void MarkPaid()
        {
            if (OrderStatus == OrderStatus.Placed)
            {
                OrderStatus = OrderStatus.Paid;
                RaiseDomainEvent(new OrderPaid(BuyerId, CheckoutId, DateTime.UtcNow));
            }
            else
                throw new InvalidOperationException("Order isn't in placed status");
        }

        public void Fulfill()
        {
            if (OrderStatus == OrderStatus.Paid)
            {
                OrderStatus = OrderStatus.Fulfilled;
                RaiseDomainEvent(new OrderFulfilled(BuyerId, CheckoutId, DateTime.UtcNow));
            }
            else
                throw new InvalidOperationException("Order isn't in paid status");
        }

        public void Cancel()
        {
            if (OrderStatus == OrderStatus.Placed)
            {
                OrderStatus = OrderStatus.Cancelled;
                RaiseDomainEvent(new OrderCancelled(BuyerId, CheckoutId, NeedRefund, DateTime.UtcNow));
            }
            else if (OrderStatus == OrderStatus.Paid)
            {
                OrderStatus = OrderStatus.Cancelled;
                NeedRefund = true;
                RaiseDomainEvent(new OrderCancelled(BuyerId, CheckoutId, NeedRefund, DateTime.UtcNow));
            }
            else
                throw new InvalidOperationException("Order isn't in placed or paid status");
        }

    }

    public enum OrderStatus
    {
        Placed,
        Paid,
        Fulfilled,
        Cancelled
    }

    public readonly record struct OrderLineRestore(Guid GameId, string Title, long UnitPrice, int Quantity);
}
