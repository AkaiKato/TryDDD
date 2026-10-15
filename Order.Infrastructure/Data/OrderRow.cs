using Order.Domain.Aggregates;

namespace Order.Infrastructure.Data
{
    public class OrderRow
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public Guid CheckoutId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool NeedRefund { get; set; }
        public List<OrderLineRow> OrderLines { get; set; } = [];
    }
}
