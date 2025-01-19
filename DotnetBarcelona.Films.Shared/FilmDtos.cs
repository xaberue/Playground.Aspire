namespace DotnetBarcelona.Films.Shared;

public record FilmDto(int Id, string Name, DateTime ReleaseDate, FilmCategory[] Categories, int[] Cast);

public record FilmCreation
{
    public string Name { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; } = new DateTime(2000, 1, 1);
    public FilmCategory[]? Categories { get; set; } = [];
    public int[]? Cast { get; set; } = [];
}