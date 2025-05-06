using GameStore.Infrastructure.Models.Application;
using System.Linq.Expressions;

namespace GameStore.Infrastructure.Stores.Abstractions;

public interface IQueryStore<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    Task<IEnumerable<TEntity>> GetAsync();
    
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<TEntity?> GetAsync(TKey id);
    
    Task<bool> ExistsAsync(TKey id);
    
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}
