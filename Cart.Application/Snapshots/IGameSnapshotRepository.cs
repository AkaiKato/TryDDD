namespace Cart.Application.Snapshots
{
    public sealed record GameSnapshotData(Guid GameId, string Title, long Price, bool ForSale);

    public interface IGameSnapshotRepository
    {
        Task<GameSnapshotData?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken);

        Task ApplyPublishedAsync(Guid gameId, string title, long price, CancellationToken cancellationToken);

        Task ApplyPriceChangedAsync(Guid gameId, long price, CancellationToken cancellationToken);

        Task ApplyRenamedAsync(Guid gameId, string title, CancellationToken cancellationToken);

        Task ApplyArchivedAsync(Guid gameId, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
