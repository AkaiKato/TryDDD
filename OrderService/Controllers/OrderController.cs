using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands;
using Order.Application.Queries;
using OrderService.DTOs;
using Shared.Auth.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/order")]
    [Authorize]
    public class OrderController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
    {
        [HttpPost("CreateLicenseKey")]
        public async Task<IActionResult> CreateLicenseKey([FromBody] CreateLicenseKeyDto data, CancellationToken ct)
        {
            var licenseKeyId = await mediator.Send(new CreateLicenseKeyCommand(data.GameId, data.Code), ct);
            return Ok(new { LicenseKeyId = licenseKeyId });
        }

        [HttpPut("Pay")]
        public async Task<IActionResult> Pay([FromQuery] Guid orderId, CancellationToken ct)
        {
            await mediator.Send(new MarkPaidCommand(currentUserService.UserId, orderId), ct);
            return Ok();
        }

        [HttpPut("Fulfill")]
        public async Task<IActionResult> Fulfill([FromQuery] Guid orderId, CancellationToken ct)
        {
            await mediator.Send(new FulfillOrderCommand(currentUserService.UserId, orderId), ct);
            return Ok();
        }

        [HttpPut("Cancel")]
        public async Task<IActionResult> Cancel([FromQuery] Guid orderId, CancellationToken ct)
        {
            await mediator.Send(new CancelOrderCommand(currentUserService.UserId, orderId), ct);
            return Ok();
        }

        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetOrder([FromQuery] Guid orderId, CancellationToken ct)
        {
            var order = await mediator.Send(new GetOrderQuery(currentUserService.UserId, orderId), ct);
            return Ok(order);
        }
    }
}
