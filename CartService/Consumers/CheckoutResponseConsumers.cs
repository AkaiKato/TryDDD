using Cart.Domain.Repositories;
using MassTransit;
using Order.Domain.DomainEvent;

namespace CartService.Consumers
{
    public class CheckoutAcceptedConsumer(ICartRepository carts) : IConsumer<CheckoutAccepted>
    {
        public async Task Consume(ConsumeContext<CheckoutAccepted> context)
        {
            var cart = await carts.GetByBuyerIdAsync(context.Message.BuyerId, context.CancellationToken);
            if (cart is null)
                return;

            cart.CompleteCheckout(context.Message.CheckoutId);
            await carts.UpdateAsync(cart, context.CancellationToken);
            await carts.SaveChangesAsync(context.CancellationToken);
        }
    }

    public class CheckoutRejectedConsumer(ICartRepository carts) : IConsumer<CheckoutRejected>
    {
        public async Task Consume(ConsumeContext<CheckoutRejected> context)
        {
            var cart = await carts.GetByBuyerIdAsync(context.Message.BuyerId, context.CancellationToken);
            if (cart is null)
                return;

            cart.FailCheckout(context.Message.CheckoutId);
            await carts.UpdateAsync(cart, context.CancellationToken);
            await carts.SaveChangesAsync(context.CancellationToken);
        }
    }
}
