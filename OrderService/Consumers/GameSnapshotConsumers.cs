using Catalog.Domain.DomainEvent;
using MassTransit;
using Order.Application.Snapshots;

namespace OrderService.Consumers
{
    public class GamePublishedConsumer(IGameSnapshotRepository snapshots) : IConsumer<GamePublished>
    {
        public async Task Consume(ConsumeContext<GamePublished> context)
        {
            await snapshots.ApplyPublishedAsync(
                context.Message.GameId,
                context.Message.Name,
                context.Message.Price,
                context.CancellationToken);
            await snapshots.SaveChangesAsync(context.CancellationToken);
        }
    }

    public class PriceChangedConsumer(IGameSnapshotRepository snapshots) : IConsumer<PriceChanged>
    {
        public async Task Consume(ConsumeContext<PriceChanged> context)
        {
            await snapshots.ApplyPriceChangedAsync(
                context.Message.GameId,
                context.Message.NewPrice,
                context.CancellationToken);
            await snapshots.SaveChangesAsync(context.CancellationToken);
        }
    }

    public class GameRenamedConsumer(IGameSnapshotRepository snapshots) : IConsumer<GameRenamed>
    {
        public async Task Consume(ConsumeContext<GameRenamed> context)
        {
            await snapshots.ApplyRenamedAsync(
                context.Message.GameId,
                context.Message.NewName,
                context.CancellationToken);
            await snapshots.SaveChangesAsync(context.CancellationToken);
        }
    }

    public class GameArchivedConsumer(IGameSnapshotRepository snapshots) : IConsumer<GameArchived>
    {
        public async Task Consume(ConsumeContext<GameArchived> context)
        {
            await snapshots.ApplyArchivedAsync(context.Message.GameId, context.CancellationToken);
            await snapshots.SaveChangesAsync(context.CancellationToken);
        }
    }
}
