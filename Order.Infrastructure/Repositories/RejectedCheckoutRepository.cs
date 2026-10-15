using Microsoft.EntityFrameworkCore;
using Order.Application.Checkouts;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class RejectedCheckoutRepository(OrderDbContext context) : IRejectedCheckoutRepository
    {
        public Task<bool> ExistsAsync(Guid checkoutId, CancellationToken cancellationToken = default)
        {
            if (checkoutId == Guid.Empty)
                throw new ArgumentException("CheckoutId can't be empty", nameof(checkoutId));

            return context.RejectedCheckouts
                .AsNoTracking()
                .AnyAsync(x => x.CheckoutId == checkoutId, cancellationToken);
        }

        public async Task AddAsync(Guid checkoutId, CancellationToken cancellationToken = default)
        {
            if (checkoutId == Guid.Empty)
                throw new ArgumentException("CheckoutId can't be empty", nameof(checkoutId));

            await context.RejectedCheckouts.AddAsync(new RejectedCheckoutRow
            {
                CheckoutId = checkoutId
            }, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await OrderDbSave.SaveAsync(context, cancellationToken);
        }
    }
}
