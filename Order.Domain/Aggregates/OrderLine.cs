using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates
{
    public class OrderLine
    {
        public Guid GameId { get; private set; }
        public string Title { get; private set; } = null!;
        public Money Price { get; private set; }
        public int Quantity { get; private set; }
        public long TotalForLine => Price.Amount * Quantity;

        private OrderLine(Guid gameId, string title, Money price, int quantity)
        {
            GameId = gameId;
            Title = title;
            Price = price;
            Quantity = quantity;
        }

        internal static OrderLine Create(Guid gameId, string title, long unitPrice, int quantity)
            => Build(gameId, title, unitPrice, quantity);

        internal static OrderLine Restore(Guid gameId, string title, long unitPrice, int quantity)
            => Build(gameId, title, unitPrice, quantity);

        private static OrderLine Build(Guid gameId, string title, long unitPrice, int quantity)
        {
            if (gameId == Guid.Empty)
                throw new InvalidOperationException("GameId can't be empty");

            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException("Title can't be empty");

            if (quantity < 1)
                throw new InvalidOperationException("Quantity can't be less than 1");

            if (quantity > 10)
                throw new InvalidOperationException("Quantity can't be more than 10");

            return new OrderLine(gameId, title.Trim(), Money.Create(unitPrice, Currency.RUB), quantity);
        }


    }
}
