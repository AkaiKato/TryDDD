namespace Order.Application.Checkouts
{
    public interface IRejectedCheckoutRepository
    {
        Task<bool> ExistsAsync(Guid checkoutId, CancellationToken cancellationToken = default);
        Task AddAsync(Guid checkoutId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
