using DotnetBarcelona.Actors.Shared;

namespace DotnetBarcelona.FilmsManager.WebUI.Services;

public class ActorsApiService(HttpClient filmsHttpClient)
{

    public async Task<List<ActorDto>?> GetAllActorsAsync(CancellationToken cancellationToken = default)
    {
        List<ActorDto>? actors = null;

        await foreach (var film in filmsHttpClient.GetFromJsonAsAsyncEnumerable<ActorDto>("/actors", cancellationToken))
        {
            if (film is not null)
            {
                actors ??= [];
                actors.Add(film);
            }
        }

        return actors;
    }
}
