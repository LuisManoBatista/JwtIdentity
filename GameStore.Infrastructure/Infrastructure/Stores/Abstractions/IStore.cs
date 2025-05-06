using GameStore.Infrastructure.Models.Application;

namespace GameStore.Infrastructure.Stores.Abstractions;

public interface IStore<TEntity, TKey> : IQueryStore<TEntity, TKey>, ICommandStore<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
}
