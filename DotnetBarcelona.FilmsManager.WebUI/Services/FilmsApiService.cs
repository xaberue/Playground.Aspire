using DotnetBarcelona.Films.Shared;
using DotnetBarcelona.FilmsManager.Shared;

namespace DotnetBarcelona.FilmsManager.WebUI.Services;

public class FilmsApiService(HttpClient filmsHttpClient)
{
    public async Task<IQueryable<FilmGridDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<FilmGridDto>? films = null;

        await foreach (var film in filmsHttpClient.GetFromJsonAsAsyncEnumerable<FilmGridDto>("/films", cancellationToken))
        {
            if (film is not null)
            {
                films ??= [];
                films.Add(film);
            }
        }

        return films?.AsQueryable();
    }

    public async Task<FilmDetailDto> GetAsync(int filmId, CancellationToken cancellationToken = default)
    {
        return await filmsHttpClient.GetFromJsonAsync<FilmDetailDto>($"/film/{filmId}", cancellationToken);
    }

    public async Task RegisterAsync(FilmCreation filmCreation, CancellationToken cancellationToken = default)
    {
        await filmsHttpClient.PostAsJsonAsync("/film", filmCreation, cancellationToken);
    }

    public async Task RemoveAsync(int filmId, CancellationToken cancellationToken = default)
    {
        await filmsHttpClient.DeleteAsync($"/film/{filmId}", cancellationToken);
    }

}
