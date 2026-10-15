using Cart.Domain.Aggregates;

namespace Cart.Infrastructure.Data
{
    public class CartRow
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public Guid CheckoutId { get; set; }
        public CartStatus CartStatus { get; set; }
        public List<CartItemRow> CartItems { get; set; } = [];
    }
}
