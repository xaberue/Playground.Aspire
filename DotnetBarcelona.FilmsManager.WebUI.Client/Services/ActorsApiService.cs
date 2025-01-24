using DotnetBarcelona.Actors.Shared;
using System.Net.Http.Json;

namespace DotnetBarcelona.FilmsManager.WebUI.Client.Services;

public class ActorsApiService(HttpClient actorsHttpClient)
{

    public async Task<IEnumerable<ActorDto>> GetAllActorsAsync(CancellationToken cancellationToken = default)
    {
        var actors = await actorsHttpClient.GetFromJsonAsync<IEnumerable<ActorDto>>("/api/actors", cancellationToken);

        return actors!;
    }
}
