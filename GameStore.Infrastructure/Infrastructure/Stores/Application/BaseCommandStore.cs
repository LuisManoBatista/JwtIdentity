using GameStore.Infrastructure.Models.Application;
using GameStore.Infrastructure.Stores.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Stores.Application;

internal class BaseCommandStore<TEntity, TKey> : ICommandStore<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseCommandStore(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(TKey id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
