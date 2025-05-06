using GameStore.Infrastructure.Models.Application;
using GameStore.Infrastructure.Stores.Abstractions;


namespace GameStore.Infrastructure.Stores.Application;

internal class GamesStore(IGamesQueryStore queryStore, IGamesCommandStore commandStore) 
    : BaseStore<Game, int>(queryStore, commandStore), IGamesStore
{
}
