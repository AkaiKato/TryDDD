namespace Cart.Infrastructure.Data
{
    public class CartItemRow
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string Title { get; set; } = null!;
        public long Price { get; set; }
        public int Quantity { get; set; }
        public Guid CartId { get; set; }
        public CartRow Cart { get; set; } = null!;
    }
}
