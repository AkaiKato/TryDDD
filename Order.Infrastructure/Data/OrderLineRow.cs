namespace Order.Infrastructure.Data
{
    public class OrderLineRow
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string Title { get; set; } = null!;
        public long Price { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public OrderRow Order { get; set; } = null!;
    }
}
