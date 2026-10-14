using Cart.Domain.ValueObjects;

namespace Cart.Domain.Aggregates
{
    public class CartItem
    {
        public Guid GameId { get; private set; }

        public string Title { get; private set; } = null!;

        public Money Price { get; private set; }

        public int Quantity { get; private set; }

        private CartItem(Guid gameId, string title, Money price, int quantity)
        {
            GameId = gameId;
            Title = title;
            Price = price;
            Quantity = quantity;
        }

        internal static CartItem Create(Guid gameId, string title, long unitPrice, int quantity)
            => Build(gameId, title, unitPrice, quantity);

        internal static CartItem Restore(Guid gameId, string title, long unitPrice, int quantity)
            => Build(gameId, title, unitPrice, quantity);

        private static CartItem Build(Guid gameId, string title, long unitPrice, int quantity)
        {
            if (gameId == Guid.Empty)
                throw new InvalidOperationException("GameId cannot be empty");

            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException("Title cannot be empty");

            if (quantity < 1)
                throw new InvalidOperationException("Quantity can't be less than 1");

            if (quantity > 10)
                throw new InvalidOperationException("Quantity can't be more than 10");

            return new CartItem(gameId, title.Trim(), Money.Create(unitPrice, Currency.RUB), quantity);
        }

        internal void ChangeQuantity(int quantity)
        {
            if(quantity > 10)
            {
                throw new InvalidOperationException("Quantity can't be more than 10");
            }
            if(quantity < 1)
            {
                throw new InvalidOperationException("Quantity can't be less than 1");
            }
            Quantity = quantity;
        }
    }
}
