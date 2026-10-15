using MassTransit;
using OrderAggregate = Order.Domain.Aggregates.Order;

namespace Order.Application.Commands
{
    internal static class OrderEventPublisher
    {
        public static async Task PublishAsync(
            OrderAggregate order,
            IPublishEndpoint publishEndpoint,
            CancellationToken cancellationToken)
        {
            foreach (var domainEvent in order.DomainEvents)
                await publishEndpoint.Publish(domainEvent, domainEvent.GetType(), cancellationToken);
        }
    }
}
