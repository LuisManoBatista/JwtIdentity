using GameStore.Infrastructure.Models.Application;


namespace GameStore.Infrastructure.Stores.Abstractions;

public interface ICommandStore<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey id);
}
