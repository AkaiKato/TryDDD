using Cart.Domain.Building_blocks;
using Cart.Domain.DomainEvent;

namespace Cart.Domain.Aggregates
{
    public class Cart : AggregateRoot<Guid>
    {
        private List<CartItem> CartItems { get; set; } = [];

        public Guid BuyerId { get; private set; }
        public CartStatus CartStatus { get; private set; }
        public Guid CheckoutId { get; private set; }
        public long TotalPrice => CartItems.Sum(item => item.Price.Amount * item.Quantity);
        public IReadOnlyCollection<CartItem> CartItemsReadOnly => CartItems.AsReadOnly();

        private Cart(Guid id, Guid buyerId) : base(id)
        {
            BuyerId = buyerId;
            CartStatus = CartStatus.Open;
        }

        private Cart(Guid id, Guid buyerId, CartStatus cartStatus, Guid checkoutId, List<CartItem> cartItems) : base(id)
        {
            BuyerId = buyerId;
            CartStatus = cartStatus;
            CartItems = cartItems?.ToList() ?? [];
            CheckoutId = checkoutId;
        }

        public static Cart Create(Guid buyerId)
        {
            var cart = new Cart(Guid.NewGuid(), buyerId);
            return cart;
        }

        public static Cart Restore(
            Guid id,
            Guid buyerId,
            CartStatus cartStatus,
            Guid checkoutId,
            IReadOnlyList<CartLine> lines)
        {
            if (lines is null)
                throw new ArgumentNullException(nameof(lines));

            if (cartStatus == CartStatus.CheckoutPending && checkoutId == Guid.Empty)
                throw new InvalidOperationException("CheckoutId can't be empty");

            if (cartStatus == CartStatus.CheckoutPending && lines.Count == 0)
                throw new InvalidOperationException("Can't restore an empty cart in checkout");

            if (lines.Select(line => line.GameId).Distinct().Count() != lines.Count)
                throw new InvalidOperationException("Cart can't contain two lines for the same game");

            var items = lines
                .Select(line => CartItem.Restore(line.GameId, line.Title, line.UnitPrice, line.Quantity))
                .ToList();

            return new Cart(id, buyerId, cartStatus, checkoutId, items);
        }

        public void AddItem(Guid gameId, string title, long unitPrice, int quantity)
        {
            if (CartStatus == CartStatus.Open)
            {
                if (quantity < 1)
                    throw new InvalidOperationException("Cannot Add negative game quantity");
                if (quantity > 10)
                    throw new InvalidOperationException("Quantity cannot be more than 10");
                if (gameId == Guid.Empty)
                    throw new InvalidOperationException("GameId cannot be empty");
                if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(title))
                    throw new InvalidOperationException("Title cannot be empty");
                if (unitPrice <= 0)
                    throw new InvalidOperationException("UnitPrice cannot be empty");

                var cartItem = CartItems.FirstOrDefault(x => x.GameId == gameId);

                if (cartItem == null)
                {
                    CartItems.Add(CartItem.Create(gameId, title.Trim(), unitPrice, quantity));
                }
                else
                {
                    cartItem.ChangeQuantity(cartItem.Quantity + quantity);
                }
            }
            else
            {
                throw new InvalidOperationException("Can't change cart when cart status not open");
            }
        }

        public void ChangeQuantity(Guid gameId, int quantity)
        {
            if (CartStatus == CartStatus.Open)
            {
                var cartItem = CartItems.FirstOrDefault(x => x.GameId == gameId);
                if (cartItem != null)
                {
                    if (quantity > 0)
                    {
                        cartItem.ChangeQuantity(quantity);
                    }
                    else if (quantity <= 0)
                    {
                        RemoveItem(gameId);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Can't change cart when cart status not open");
            }
        }

        public void RemoveItem(Guid gameId)
        {
            if (CartStatus == CartStatus.Open)
            {
                var removedItem = CartItems.FirstOrDefault(x => x.GameId == gameId);
                if (removedItem != null)
                    CartItems.Remove(removedItem);
            }
            else
            {
                throw new InvalidOperationException("Can't change cart when cart status not open");
            }
        }

        public void RequestCheckout(Guid checkoutId)
        {
            if (checkoutId == Guid.Empty)
                throw new InvalidOperationException("CheckoutId can't be empty");

            if (CartStatus != CartStatus.Open)
                throw new InvalidOperationException("Can't change cart when cart status not open");

            if (CartItems.Count == 0)
                throw new InvalidOperationException("Can't checkout an empty cart");

            CheckoutId = checkoutId;
            CartStatus = CartStatus.CheckoutPending;

            var lines = CartItems
                .Select(item => new CartLine(item.GameId, item.Title, item.Price.Amount, item.Quantity))
                .ToArray();

            RaiseDomainEvent(new CheckoutRequested(CheckoutId, BuyerId, lines, DateTime.UtcNow));
        }

        public void CompleteCheckout(Guid checkoutId)
        {
            if (CartStatus == CartStatus.CheckoutPending && CheckoutId == checkoutId)
            {
                CartItems.Clear();
                CartStatus = CartStatus.Open;
            }
        }

        public void FailCheckout(Guid checkoutId)
        {
            if (CartStatus == CartStatus.CheckoutPending && CheckoutId == checkoutId)
            {
                CartStatus = CartStatus.Open;
            }
        }
    }

    public enum CartStatus
    {
        Open,
        CheckoutPending,
    }

    public readonly record struct CartLine(Guid GameId, string Title, long UnitPrice, int Quantity);
}
