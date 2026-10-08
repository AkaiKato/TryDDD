using Identity.Application.Commands;
using IdentityService.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public class IdentityController
        (IMediator mediator): ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto data, CancellationToken ct)
        {
            var accountId = await mediator.Send(new RegisterAccountCommand(data.Email, data.Password), ct);
            return Ok(new { AccountId = accountId });
        }

        [HttpPut("Block")]
        public async Task<IActionResult> Block([FromQuery] Guid Id, CancellationToken ct)
        {
            await mediator.Send(new BlockAccountCommand(Id), ct);
            return Ok("Successfully blocked");
        }

        [HttpPut("UnBlock")]
        public async Task<IActionResult> UnBlock([FromQuery] Guid Id, CancellationToken ct)
        {
            await mediator.Send(new UnBlockAccountCommand(Id), ct);
            return Ok("Successfully Unblocked");
        }
    }
}
