using Cart.Domain.Repositories;
using MassTransit;

namespace CartService.Consumers
{
    public class CheckoutAcceptedConsumers(ICartRepository cartRepository) : IConsumer
    {
        public async Task Consume(ConsumeContext context)
        {
        }
    }
    public class CheckoutRejectedConsumers(ICartRepository cartRepository) : IConsumer
    {
        public async Task Consume(ConsumeContext context)
        {
        }
    }
}
