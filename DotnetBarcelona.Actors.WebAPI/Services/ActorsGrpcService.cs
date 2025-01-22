using DotnetBarcelona.Actors.WebAPI.Infrastructure;
using DotnetBarcelona.Actors.WebAPI.Models;
using Grpc.Core;

namespace DotnetBarcelona.Actors.WebAPI.Services;

public class ActorsGrpcService : Actors.ActorsBase
{

    private readonly ActorsDbContext _actorsDbContext;

    public ActorsGrpcService(ActorsDbContext actorsDbContext)
    {
        _actorsDbContext = actorsDbContext;
    }

    public override async Task<GetResponse> Get(GetRequest request, ServerCallContext context)
    {
        var actor = await _actorsDbContext.Actors.FindAsync(request.Id);
        var response = new GetResponse();

        response.Actor = actor.ToGrpcModel();

        return response;
    }
}
