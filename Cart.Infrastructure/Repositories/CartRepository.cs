using Cart.Domain.Aggregates;
using Cart.Domain.Repositories;
using Cart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Repositories
{
    public class CartRepository(CartDbContext context) : ICartRepository
    {
        public async Task<Domain.Aggregates.Cart?> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default)
        {
            var cart = await context.Carts.Include(x => x.CartItems).AsNoTracking()
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId, cancellationToken);

            return cart == null ? null : ToCart(cart);
        }

        public async Task AddAsync(Domain.Aggregates.Cart cart, CancellationToken cancellationToken = default)
        {
            context.Carts.Add(ToRow(cart));
        }

        public async Task UpdateAsync(Domain.Aggregates.Cart cart, CancellationToken cancellationToken = default)
        {
            var row = await context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.Id == cart.Id, cancellationToken);

            if (row == null)
                throw new InvalidOperationException($"Cart with ID {cart.Id} not found.");

            row.CartStatus = cart.CartStatus;
            row.CheckoutId = cart.CheckoutId;

            var incomingGameIds = cart.CartItemsReadOnly.Select(item => item.GameId).ToHashSet();
            var removed = row.CartItems.Where(item => !incomingGameIds.Contains(item.GameId)).ToList();
            foreach (var item in removed)
                context.Remove(item);

            foreach (var item in cart.CartItemsReadOnly)
            {
                var existing = row.CartItems.FirstOrDefault(rowItem => rowItem.GameId == item.GameId);
                if (existing == null)
                {
                    row.CartItems.Add(ToItemRow(cart.Id, item));
                    continue;
                }

                existing.Title = item.Title;
                existing.Price = item.Price.Amount;
                existing.Quantity = item.Quantity;
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        private static CartRow ToRow(Domain.Aggregates.Cart cart)
        {
            List<CartItemRow> cartRows = [];

            foreach (var item in cart.CartItemsReadOnly)
                cartRows.Add(ToItemRow(cart.Id, item));

            return new CartRow
            {
                Id = cart.Id,
                CartStatus = cart.CartStatus,
                BuyerId = cart.BuyerId,
                CheckoutId = cart.CheckoutId,
                CartItems = cartRows,
            };
        }

        private static Domain.Aggregates.Cart ToCart(CartRow row)
        {
            List<CartLine> cartLines = [];

            foreach (var item in row.CartItems)
            {
                cartLines.Add(
                    new CartLine
                    {
                        GameId = item.GameId,
                        UnitPrice = item.Price,
                        Quantity = item.Quantity,
                        Title = item.Title
                    });
            }

            return Domain.Aggregates.Cart.Restore(row.Id, row.BuyerId, row.CartStatus, row.CheckoutId, cartLines);
        }

        private static CartItemRow ToItemRow(Guid cartId, CartItem item) => new()
        {
            Id = Guid.NewGuid(),
            CartId = cartId,
            GameId = item.GameId,
            Price = item.Price.Amount,
            Quantity = item.Quantity,
            Title = item.Title
        };

    }
}
