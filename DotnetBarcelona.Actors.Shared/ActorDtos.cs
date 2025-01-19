namespace DotnetBarcelona.Actors.Shared;

public record ActorDto(int Id, string Name, DateTime DateOfBirth)
{
    public override string ToString() => $"{Name} - ({DateOfBirth.ToShortDateString()})";
};

public record ActorCreation(string Name, DateTime DateOfBirth, string Nationality);
