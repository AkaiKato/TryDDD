using Cart.Application.Commands;
using Cart.Application.Queries;
using CartService.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth.Services;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController(
        IMediator mediator, 
        ICurrentUserService currentUserService): ControllerBase
    {
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto data, CancellationToken ct)
        {
            await mediator.Send(new AddToCartCommand(currentUserService.UserId, data.GameId, data.Quantity), ct);
            return Ok();
        }

        [HttpPut("UpdateCartItem")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto data, CancellationToken ct)
        {
            var totalPrice = await mediator.Send(
                new UpdateCartItemCommand(currentUserService.UserId, data.GameId, data.Quantity), ct);
            return Ok(new { TotalPrice = totalPrice });
        }

        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart(CancellationToken ct)
        {
            var cart = await mediator.Send(new GetCartQuery(currentUserService.UserId), ct);
            return Ok(cart);
        }
    }
}
