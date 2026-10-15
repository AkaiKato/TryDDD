namespace Cart.Domain.Repositories
{
    public interface ICartRepository
    {
        Task AddAsync(Aggregates.Cart cart, CancellationToken cancellationToken = default);
        Task UpdateAsync(Aggregates.Cart cart, CancellationToken cancellationToken = default);
        Task<Aggregates.Cart?> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
