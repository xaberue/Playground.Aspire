using DotnetBarcelona.Films.Shared;

namespace DotnetBarcelona.Films.WebAPI.Models;

public static class FilmExtensions
{
    public static FilmDto ToDto(this Film entity) =>
        new FilmDto(entity.Id, entity.Name, entity.ReleaseDate, entity.Categories, entity.Cast);

    public static FilmModel ToGrpcModel(this Film filmEntity)
    {
        var model = new FilmModel
        {
            Id = filmEntity.Id,
            Name = filmEntity.Name,
            ReleaseDate = filmEntity.ReleaseDate.ToString()
        };

        model.Categories.AddRange(filmEntity.Categories.Select(y => (int)y));
        model.Cast.AddRange(filmEntity.Cast);

        return model;
    }

}
