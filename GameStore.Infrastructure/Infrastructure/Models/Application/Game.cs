namespace GameStore.Infrastructure.Models.Application;

public class Game : IEntity<int>
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Genre { get; set; }
    
    public decimal Price { get; set; }
    
    public DateTime ReleaseDate { get; set; }
    
    public required string ImageUri { get; set; }
}