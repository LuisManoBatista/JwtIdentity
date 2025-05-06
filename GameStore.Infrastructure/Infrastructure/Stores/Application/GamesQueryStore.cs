using GameStore.Infrastructure.Models.Application;
using GameStore.Infrastructure.Stores.Abstractions;


namespace GameStore.Infrastructure.Stores.Application;

internal class GamesQueryStore(ApplicationDbContext context) : BaseQueryStore<Game, int>(context), IGamesQueryStore
{
}
