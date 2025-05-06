using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Entities;

public record CreateGameDto(
    [Required][StringLength(50)] string Name,
    [Required][StringLength(20)] string Genre,
    [Range(1, 100)] decimal Price,
    DateTime ReleaseDate,
    [Url][StringLength(100)]string ImageUri
);
