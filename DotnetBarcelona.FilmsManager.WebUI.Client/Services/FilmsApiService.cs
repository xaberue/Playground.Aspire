using DotnetBarcelona.Films.Shared;
using DotnetBarcelona.FilmsManager.Shared;
using System.Net.Http.Json;

namespace DotnetBarcelona.FilmsManager.WebUI.Client.Services;

public class FilmsApiService(HttpClient filmsHttpClient)
{
    public async Task<IEnumerable<FilmGridDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var films = await filmsHttpClient.GetFromJsonAsync<IEnumerable<FilmGridDto>>("/api/films", cancellationToken);

        return films!;
    }

    public async Task<FilmDetailDto?> GetAsync(int filmId, CancellationToken cancellationToken = default)
    {
        return await filmsHttpClient.GetFromJsonAsync<FilmDetailDto>($"/api/film/{filmId}", cancellationToken);
    }

    public async Task RegisterAsync(FilmCreation filmCreation, CancellationToken cancellationToken = default)
    {
        await filmsHttpClient.PostAsJsonAsync("/api/film", filmCreation, cancellationToken);
    }

    public async Task RemoveAsync(int filmId, CancellationToken cancellationToken = default)
    {
        await filmsHttpClient.DeleteAsync($"/api/film/{filmId}", cancellationToken);
    }

}
