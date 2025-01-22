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

    public override Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        var films = _filmsDbContext.Films.Select(x => x.ToGrpcModel()).ToList();
        var response = new GetAllResponse();

        response.Films.AddRange(films);

        return Task.FromResult(response);
    }

}
