using Catalog.Application.Commands;
using Catalog.Application.Queries;
using CatalogService.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    public class CatalogController
        (IMediator mediator): ControllerBase
    {
        [Authorize]
        [HttpPost("CreateGame")]
        public async Task<IActionResult> CreateGame([FromBody]CreateGameDto data, CancellationToken ct)
        {
            var gameId = await mediator.Send(new CreateGameCommand(data.Title, data.Price), ct);
            return Ok(new { GameId = gameId });
        }

        [Authorize]
        [HttpPut("RenameGame")]
        public async Task<IActionResult> RenameGame([FromBody] RenameGameDto data, CancellationToken ct)
        {
            await mediator.Send(new RenameGameCommand(data.GameId, data.NewTitle), ct);
            return Ok("Game renamed successfully");
        }

        [Authorize]
        [HttpPut("ChangePrice")]
        public async Task<IActionResult> ChangePrice([FromBody] ChangePriceDto data, CancellationToken ct)
        {
            await mediator.Send(new ChangePriceCommand(data.GameId, data.NewPrice), ct);
            return Ok("Game price changed successfully");
        }

        [Authorize]
        [HttpPut("PublishGame")]
        public async Task<IActionResult> PublishGame([FromQuery] Guid GameId, CancellationToken ct)
        {
            await mediator.Send(new PublishGameCommand(GameId), ct);
            return Ok("Game published successfully");
        }

        [Authorize]
        [HttpPut("ArchiveGame")]
        public async Task<IActionResult> ArchiveGame([FromQuery] Guid GameId, CancellationToken ct)
        {
            await mediator.Send(new ArchiveGameCommand(GameId), ct);
            return Ok("Game archived successfully");
        }

        [HttpPut("GetAllGames")]
        public async Task<IActionResult> GetAllGames(CancellationToken ct)
        {
            var games = await mediator.Send(new GetAllGamesQuery(), ct);
            return Ok(games);
        }
    }
}
