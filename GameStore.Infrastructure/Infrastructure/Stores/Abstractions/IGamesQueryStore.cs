using GameStore.Infrastructure.Models.Application;

namespace GameStore.Infrastructure.Stores.Abstractions;

public interface IGamesQueryStore : IQueryStore<Game, int>
{
}
