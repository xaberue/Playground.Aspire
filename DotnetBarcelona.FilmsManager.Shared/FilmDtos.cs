using DotnetBarcelona.Actors.Shared;
using DotnetBarcelona.Films.Shared;

namespace DotnetBarcelona.FilmsManager.Shared;

public record FilmGridDto(int Id, string Name, int YearReleased, string[] Categories, string[] Cast);

public record FilmDetailDto(int Id, string Name, int YearReleased, string[] Categories, IEnumerable<ActorDto> Cast);