using GameStore.Infrastructure.Models.Application;
using GameStore.Infrastructure.Stores.Abstractions;


namespace GameStore.Infrastructure.Stores.Application;

internal class GamesCommandStore(ApplicationDbContext context) : BaseCommandStore<Game, int>(context), IGamesCommandStore
{
}
