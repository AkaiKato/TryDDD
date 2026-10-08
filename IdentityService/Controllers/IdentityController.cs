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
    }
}
