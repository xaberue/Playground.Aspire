using DotnetBarcelona.Films.Shared;

namespace DotnetBarcelona.Films.WebAPI.Models;

public static class FilmExtensions
{
    public static FilmDto ToDto(this Film entity) =>
        new FilmDto(entity.Id, entity.Name, entity.ReleaseDate, entity.Categories, entity.Cast);

}
