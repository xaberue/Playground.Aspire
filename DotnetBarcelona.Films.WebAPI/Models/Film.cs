using DotnetBarcelona.Films.Shared;

namespace DotnetBarcelona.Films.WebAPI.Models;

public class Film
{

    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime ReleaseDate { get; init; }
    public FilmCategory[] Categories { get; init; } = [];
    public int[] Cast { get; init; } = [];


    public Film(string name, DateTime releaseDate)
    {
        Name = name;
        ReleaseDate = releaseDate;

    }

    public Film(int id, string name, DateTime releaseDate)
        : this(name, releaseDate)
    {
        Id = id;
    }

    public Film(string name, DateTime releaseDate, FilmCategory[] categories, int[] cast)
        : this(name, releaseDate)
    {
        Categories = categories;
        Cast = cast;
    }

    public Film(int id, string name, DateTime releaseDate, FilmCategory[] categories, int[] cast)
        : this (id, name, releaseDate)
    {
        Categories = categories;
        Cast = cast;
    }

}
