using Identity.Domain.DomainEvent;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace IdentityService.Background
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
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task DispatchOnce(CancellationToken stoppingToken)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var message = await context.OutboxMessages
                .Where(x => x.ProcessedOn == null)
                .OrderBy(x => x.OccurredOn)
                .FirstOrDefaultAsync(stoppingToken);

            if(message == null)
                return;

            if (message.Type == typeof(AccountRegistered).FullName)
            {
                var body = JsonSerializer.Deserialize<AccountRegistered>(message.Json)
                    ?? throw new InvalidOperationException("Empty AccountRegistered payload");

                await publish.Publish(body, stoppingToken);
            }

            message.ProcessedOn = DateTime.UtcNow;

            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
