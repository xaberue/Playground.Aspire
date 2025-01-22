using DotnetBarcelona.Actors.Shared;

namespace DotnetBarcelona.Actors.WebAPI.Models;

public static class ActorExtensions
{
    public static ActorDto ToDto(this Actor entity)
        => new ActorDto(entity.Id, entity.Name, entity.DateOfBirth);

    public static ActorModel ToGrpcModel(this Actor entity)
    {
        return new ActorModel()
        {
            Id = entity.Id,
            Name = entity.Name,
            DateOfBirth = entity.DateOfBirth.ToString()
        };
    }
}
