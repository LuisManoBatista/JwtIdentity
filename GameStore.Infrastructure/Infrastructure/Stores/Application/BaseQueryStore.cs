using GameStore.Infrastructure.Models.Application;
using GameStore.Infrastructure.Stores.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace GameStore.Infrastructure.Stores.Application;

internal abstract class BaseQueryStore<TEntity, TKey> : IQueryStore<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    protected readonly DbContext _context;

    protected readonly DbSet<TEntity> _dbSet;

    protected BaseQueryStore(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public virtual async Task<TEntity?> GetAsync(TKey id)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id!.Equals(id));
    }

    public virtual async Task<bool> ExistsAsync(TKey id)
    {
        return await _dbSet.AnyAsync(e => e.Id!.Equals(id));
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}
