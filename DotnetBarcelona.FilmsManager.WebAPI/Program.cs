using DotnetBarcelona.Actors;
using DotnetBarcelona.Actors.Shared;
using DotnetBarcelona.Films;
using DotnetBarcelona.Films.Shared;
using DotnetBarcelona.FilmsManager.Shared;
using DotnetBarcelona.FilmsManager.WebAPI.Configuration;
using Grpc.Net.Client;

var builder = WebApplication.CreateBuilder(args);

var grpcEnabled = bool.Parse(builder.Configuration["EnableGrpc"]!);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var filmsApiUrl = builder.Configuration.GetConnectionString("FilmsApiUrl")
    ?? throw new ArgumentException("FilmsAPIUrl is mandatory");

var actorsApiUrl = builder.Configuration.GetConnectionString("ActorsApiUrl")
    ?? throw new ArgumentException("ActorsApiUrl is mandatory");

builder.Services.AddHttpClient(
    FilmsManagerApiConstants.FilmsApiClient,
    client =>
    {
        client.BaseAddress = new Uri(filmsApiUrl);
    });

builder.Services.AddHttpClient(
    FilmsManagerApiConstants.ActorsApiClient,
    client =>
    {
        client.BaseAddress = new Uri(actorsApiUrl);
    });

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/films", async (IHttpClientFactory httpClientFactory) =>
{
    if (grpcEnabled)
    {
        using var filmsChannel = GrpcChannel.ForAddress(filmsApiUrl);
        using var actorsChannel = GrpcChannel.ForAddress(actorsApiUrl);

        var filmsClient = new Films.FilmsClient(filmsChannel);
        var actorsClient = new Actors.ActorsClient(actorsChannel);

        var response = await filmsClient.GetAllAsync(new GetAllFilmsRequest());

        var actors = new List<ActorModel>();

        foreach (var actorId in response.Films.SelectMany(x => x.Cast).Distinct())
        {
            var actorResponse = await actorsClient.GetAsync(new GetActorRequest { Id = actorId });

            actors.Add(actorResponse.Actor);
        }

        return response.Films.Select(x => new FilmGridDto(
            x.Id,
            x.Name,
            DateTime.Parse(x.ReleaseDate).Year,
            x.Categories.Select(x => ((FilmCategory)x).ToString()).ToArray(),
            actors.Where(y => x.Cast.Contains(y.Id)).Select(y => y.Name).ToArray()
            ));
    }
    else
    {
        var filmsClient = httpClientFactory.CreateClient(FilmsManagerApiConstants.FilmsApiClient);
        var actorsClient = httpClientFactory.CreateClient(FilmsManagerApiConstants.ActorsApiClient);
        var films = await filmsClient.GetFromJsonAsync<FilmDto[]>("/films");
        var actors = new List<ActorDto>();

        foreach (var actorId in films.SelectMany(x => x.Cast).Distinct())
        {
            var actor = await actorsClient.GetFromJsonAsync<ActorDto>($"/actor/{actorId}");

            actors.Add(actor);
        }

        return films.Select(x => new FilmGridDto(
            x.Id,
            x.Name,
            x.ReleaseDate.Year,
            x.Categories.Select(x => x.ToString()).ToArray(),
            actors.Where(y => x.Cast.Contains(y.Id)).Select(y => y.Name).ToArray())
        );
    }
})
.WithName("GetAllFilms");

app.MapGet("/film/{filmId}", async (IHttpClientFactory httpClientFactory, int filmId) =>
{
    if (grpcEnabled)
    {
        using var filmsChannel = GrpcChannel.ForAddress(filmsApiUrl);
        using var actorsChannel = GrpcChannel.ForAddress(actorsApiUrl);

        var filmsClient = new Films.FilmsClient(filmsChannel);
        var actorsClient = new Actors.ActorsClient(actorsChannel);

        var response = await filmsClient.GetAsync(new GetFilmRequest { Id = filmId });

        var actors = new List<ActorModel>();

        foreach (var actorId in response.Film.Cast)
        {
            var actorResponse = await actorsClient.GetAsync(new GetActorRequest { Id = actorId });

            actors.Add(actorResponse.Actor);
        }

        return new FilmDetailDto(
            response.Film.Id,
            response.Film.Name,
            DateTime.Parse(response.Film.ReleaseDate).Year,
            response.Film.Categories.Select(x => ((FilmCategory)x).ToString()).ToArray(),
            actors.Select(x => new ActorDto(x.Id, x.Name, DateTime.Parse(x.DateOfBirth)))
        );
    }
    else
    {
        var filmsClient = httpClientFactory.CreateClient(FilmsManagerApiConstants.FilmsApiClient);
        var actorsClient = httpClientFactory.CreateClient(FilmsManagerApiConstants.ActorsApiClient);
        var film = await filmsClient.GetFromJsonAsync<FilmDto>($"/film/{filmId}");
        var actors = new List<ActorDto>();

        foreach (var actorId in film.Cast)
        {
            var actor = await actorsClient.GetFromJsonAsync<ActorDto>($"/actor/{actorId}");

            actors.Add(actor);
        }

        return new FilmDetailDto(
            film.Id,
            film.Name,
            film.ReleaseDate.Year,
            film.Categories.Select(x => x.ToString()).ToArray(),
            actors);
    }
})
.WithName("GetFilm");

app.MapPost("/film", async (IHttpClientFactory httpClientFactory, FilmCreation filmCreation) =>
{
    if (grpcEnabled)
    {
        using var filmsChannel = GrpcChannel.ForAddress(filmsApiUrl);
        
        var filmsClient = new Films.FilmsClient(filmsChannel);

        var createRequest = new CreateFilmRequest
        {
            Name = filmCreation.Name,
            ReleaseDate = filmCreation.ReleaseDate.ToString()
        };
        createRequest.Categories.AddRange(filmCreation.Categories.Select(x => (int)x));
        createRequest.Cast.AddRange(filmCreation.Cast);

        await filmsClient.RegisterAsync(createRequest);

        return Results.Created();
    }
    else
    {
        var filmsClient = httpClientFactory.CreateClient(FilmsManagerApiConstants.FilmsApiClient);

        await filmsClient.PostAsJsonAsync("/film", filmCreation);

        return Results.Created();
    }
})
.WithName("RegisterFilm");

app.MapDelete("/film/{filmId}", async (IHttpClientFactory httpClientFactory, int filmId) =>
{
    if (grpcEnabled)
    {
        using var filmsChannel = GrpcChannel.ForAddress(filmsApiUrl);

        var filmsClient = new Films.FilmsClient(filmsChannel);

        var deleteRequest = new DeleteFilmRequest { Id = filmId };

        await filmsClient.DeleteAsync(deleteRequest);
    }
    else
    {
        var filmsClient = httpClientFactory.CreateClient(FilmsManagerApiConstants.FilmsApiClient);

        await filmsClient.DeleteAsync($"/film/{filmId}");
    }

    return Results.NoContent();
});

app.MapGet("/actors", async (IHttpClientFactory httpClientFactory) =>
{
    if (grpcEnabled)
    {
        using var actorsChannel = GrpcChannel.ForAddress(actorsApiUrl);

        var actorsClient = new Actors.ActorsClient(actorsChannel);

        var response = await actorsClient.GetAllAsync(new GetAllActorsRequest());

        return response.Actors.Select(x => new ActorDto(
            x.Id,
            x.Name,
            DateTime.Parse(x.DateOfBirth)
            )).ToArray();
    }
    else
    {
        var actorsClient = httpClientFactory.CreateClient(FilmsManagerApiConstants.ActorsApiClient);

        return await actorsClient.GetFromJsonAsync<ActorDto[]>("/actors");
    }
})
.WithName("GetAllActors");

app.MapDefaultEndpoints();

app.Run();