namespace GameStore.Infrastructure.Models.Application;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
