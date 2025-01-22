using DotnetBarcelona.Films.Shared;
using DotnetBarcelona.Films.WebAPI.Infrastructure;
using DotnetBarcelona.Films.WebAPI.Models;
using Grpc.Core;

namespace DotnetBarcelona.Films.WebAPI.Services;

public class FilmsGrpcService : Films.FilmsBase
{

    private readonly FilmsDbContext _filmsDbContext;


    public FilmsGrpcService(FilmsDbContext filmsDbContext)
    {
        _filmsDbContext = filmsDbContext;
    }


    public override Task<GetAllFilmsResponse> GetAll(GetAllFilmsRequest request, ServerCallContext context)
    {
        var films = _filmsDbContext.Films.Select(x => x.ToGrpcModel()).ToList();
        var response = new GetAllFilmsResponse();

        response.Films.AddRange(films);

        return Task.FromResult(response);
    }

    public override async Task<GetFilmResponse> Get(GetFilmRequest request, ServerCallContext context)
    {
        var film = await _filmsDbContext.Films.FindAsync(request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound, $"Film with id {request.Id} not found"));
        var response = new GetFilmResponse { Film = film.ToGrpcModel() };

        return response;
    }

    public override async Task<CreateFilmResponse> Register(CreateFilmRequest filmCreation, ServerCallContext context)
    {
        var film = new Film(filmCreation.Name, DateTime.Parse(filmCreation.ReleaseDate), filmCreation.Categories.Select(x => (FilmCategory)x).ToArray(), filmCreation.Cast.ToArray());
        var response = new CreateFilmResponse();
        
        await _filmsDbContext.Films.AddAsync(film);
        await _filmsDbContext.SaveChangesAsync();

        return response;
    }

    public override async Task<DeleteFilmResponse> Delete(DeleteFilmRequest request, ServerCallContext context)
    {
        var film = await _filmsDbContext.Films.FindAsync(request.Id) 
            ?? throw new RpcException(new Status(StatusCode.NotFound, $"Film with id {request.Id} not found"));

        _filmsDbContext.Films.Remove(film);
        await _filmsDbContext.SaveChangesAsync();

        return new DeleteFilmResponse();
    }

}
