using GameStore.Api.Entities;
using GameStore.Infrastructure.Models.Application;
using GameStore.Infrastructure.Stores.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController(IGamesStore store) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllGames()
    {
        var games = (await store.GetAsync()).Select(game => game.AsDto());
        return Ok(games);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameById(int id)
    {
        var game = await store.GetAsync(id);
        return game is not null ? Ok(game.AsDto()) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameDto gameDto)
    {
        var game = new Game
        {
            Name = gameDto.Name,
            Genre = gameDto.Genre,
            Price = gameDto.Price,
            ReleaseDate = gameDto.ReleaseDate,
            ImageUri = gameDto.ImageUri
        };

        await store.CreateAsync(game);
        return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, game.AsDto());
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] UpdateGameDto updatedGameDto)
    {
        var existingGame = await store.GetAsync(id);

        if (existingGame is null)
        {
            return NotFound();
        }

        existingGame.Name = updatedGameDto.Name;
        existingGame.Genre = updatedGameDto.Genre;
        existingGame.Price = updatedGameDto.Price;
        existingGame.ReleaseDate = updatedGameDto.ReleaseDate;
        existingGame.ImageUri = updatedGameDto.ImageUri;

        await store.UpdateAsync(existingGame);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await store.GetAsync(id);

        if (game is not null)
        {
            await store.DeleteAsync(id);
        }

        return NoContent();
    }
}