using Cart.Domain.DomainEvent;
using MassTransit;
using MediatR;
using Order.Application.Commands;

namespace OrderService.Consumers
{
    public class CheckoutRequestedConsumer(IMediator mediator) : IConsumer<CheckoutRequested>
    {
        public Task Consume(ConsumeContext<CheckoutRequested> context)
        {
            var lines = context.Message.Lines
                .Select(line => new CheckoutLine(line.GameId, line.Title, line.UnitPrice, line.Quantity))
                .ToList();

            return mediator.Send(
                new CheckoutRequestedCommand(context.Message.CheckoutId, context.Message.BuyerId, lines),
                context.CancellationToken);
        }
    }
}
