using GameStore.Infrastructure.Models.Application;

namespace GameStore.Infrastructure.Stores.Abstractions;

public interface IGamesCommandStore : ICommandStore<Game, int>
{
}
