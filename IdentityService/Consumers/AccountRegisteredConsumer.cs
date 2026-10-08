using Identity.Domain.DomainEvent;
using MassTransit;

namespace IdentityService.Consumers
{
    public class AccountRegisteredConsumer(ILogger<AccountRegisteredConsumer> logger)
        : IConsumer<AccountRegistered>
    {
        public Task Consume(ConsumeContext<AccountRegistered> context)
        {
            logger.LogInformation("Account registrated: {AccountId}", context.Message.AccountId);
            Console.WriteLine($"Account registrated: {context.Message.AccountId}");
            return Task.CompletedTask;
        }
    }
}
