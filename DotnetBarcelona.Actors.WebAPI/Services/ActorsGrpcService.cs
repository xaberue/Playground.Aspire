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


    public override Task<GetAllActorsResponse> GetAll(GetAllActorsRequest request, ServerCallContext context)
    {
        var actors = _actorsDbContext.Actors.Select(x => x.ToGrpcModel()).ToList();
        var response = new GetAllActorsResponse();

        response.Actors.AddRange(actors);

        return Task.FromResult(response);
    }

    public override async Task<GetActorResponse> Get(GetActorRequest request, ServerCallContext context)
    {
        var actor = await _actorsDbContext.Actors.FindAsync(request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound, $"Actor with id {request.Id} not found"));
        var response = new GetActorResponse();

        response.Actor = actor.ToGrpcModel();

        return response;
    }

}
