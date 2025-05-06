using GameStore.Api.Entities;
using GameStore.Infrastructure.Models.Application;
using GameStore.Infrastructure.Stores.Abstractions;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static void MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games");

        group.MapGet("/", async (IGamesStore store) =>
            (await store.GetAsync()).Select(game => game.AsDto()))
            .WithOpenApi();

        group.MapGet("/{id}", async (IGamesStore store, int id) =>
        {
            Game? game = await store.GetAsync(id);
            return game is not null ? Results.Ok(game.AsDto()) : Results.NotFound();
        })
        .WithName(GetGameEndpointName)
        .WithOpenApi();

        group.MapPost("/", async (IGamesStore store, CreateGameDto gameDto) =>
        {
            Game game = new()
            {
                Name = gameDto.Name,
                Genre = gameDto.Genre,
                Price = gameDto.Price,
                ReleaseDate = gameDto.ReleaseDate,
                ImageUri = gameDto.ImageUri
            };

            await store.CreateAsync(game);
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        }).WithOpenApi();

        group.MapPut("/{id}", async (IGamesStore store, int id, UpdateGameDto updatedGameDto) =>
        {
            Game? existingGame = await store.GetAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = updatedGameDto.Name;
            existingGame.Genre = updatedGameDto.Genre;
            existingGame.Price = updatedGameDto.Price;
            existingGame.ReleaseDate = updatedGameDto.ReleaseDate;
            existingGame.ImageUri = updatedGameDto.ImageUri;

            await store.UpdateAsync(existingGame);

            return Results.NoContent();
        }).WithOpenApi();

        group.MapDelete("/{id}", async (IGamesStore store, int id) =>
        {
            Game? game = await store.GetAsync(id);

            if (game is not null)
            {
                await store.DeleteAsync(id);
            }

            return Results.NoContent();
        }).WithOpenApi();

    }
}