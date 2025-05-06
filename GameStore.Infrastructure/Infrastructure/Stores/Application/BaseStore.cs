using GameStore.Infrastructure.Models.Application;
using GameStore.Infrastructure.Stores.Abstractions;
using System.Linq.Expressions;


namespace GameStore.Infrastructure.Stores.Application;

internal abstract class BaseStore<TEntity, TKey>(
    IQueryStore<TEntity, TKey> queryStore, 
    ICommandStore<TEntity, TKey> commandStore) : IStore<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    public Task<IEnumerable<TEntity>> GetAsync() => queryStore.GetAsync();

    public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate) =>
        queryStore.GetAsync(predicate);
    public Task<TEntity?> GetAsync(TKey id) => queryStore.GetAsync(id);
    public Task<bool> ExistsAsync(TKey id) => queryStore.ExistsAsync(id);
    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate) =>
        queryStore.ExistsAsync(predicate);
    public Task<TEntity> CreateAsync(TEntity entity) => commandStore.CreateAsync(entity);
    public Task UpdateAsync(TEntity entity) => commandStore.UpdateAsync(entity);
    public Task DeleteAsync(TKey id) => commandStore.DeleteAsync(id);
}
