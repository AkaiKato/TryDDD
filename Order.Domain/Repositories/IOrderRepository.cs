namespace Order.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Aggregates.Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Aggregates.Order order, CancellationToken cancellationToken = default);
        Task<Aggregates.Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<Aggregates.Order?> GetByCheckoutIdAsync(Guid checkoutId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
