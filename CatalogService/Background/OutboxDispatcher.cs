using Catalog.Domain.DomainEvent;
using Catalog.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Background
{
    public class OutboxDispatcher(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DispatchOnce(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task DispatchOnce(CancellationToken stoppingToken)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var message = await context.OutboxMessages
                .Where(x => x.ProcessedOn == null)
                .OrderBy(x => x.OccurredOn)
                .FirstOrDefaultAsync(stoppingToken);

            if (message == null)
                return;

            var eventType = message.Type switch
            {
                not null when message.Type == typeof(GameCreated).FullName => typeof(GameCreated),
                not null when message.Type == typeof(GameRenamed).FullName => typeof(GameRenamed),
                not null when message.Type == typeof(PriceChanged).FullName => typeof(PriceChanged),
                not null when message.Type == typeof(GamePublished).FullName => typeof(GamePublished),
                not null when message.Type == typeof(GameArchived).FullName => typeof(GameArchived),
                _ => throw new InvalidOperationException($"Unknown outbox type {message.Type}")
            };

            var body = System.Text.Json.JsonSerializer.Deserialize(message.Json, eventType)
                ?? throw new InvalidOperationException($"Empty payload for type {message.Type}");

            await publish.Publish(body, stoppingToken);

            message.ProcessedOn = DateTime.UtcNow;

            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
