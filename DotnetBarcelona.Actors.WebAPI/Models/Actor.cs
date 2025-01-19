namespace DotnetBarcelona.Actors.WebAPI.Models;

public class Actor
{

    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public string Nationality { get; init; } = null!;


    public Actor(String name, DateTime dateOfBirth, String nationality)
    {
        Name = name;
        DateOfBirth = dateOfBirth;
        Nationality = nationality;
    }

    public Actor(Int32 id, String name, DateTime dateOfBirth, String nationality)
        : this(name, dateOfBirth, nationality)
    {
        Id = id;
    }
}
