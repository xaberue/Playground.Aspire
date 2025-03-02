namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

public class Doctor
{

    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Surname { get; init; } = null!;
    public DateTime HiringDate { get; init; }


    public Doctor(string name, string surname, DateTime hiringDate)
    {
        Name = name;
        Surname = surname;
        HiringDate = hiringDate;
    }

    public Doctor(int id, string name, string surname, DateTime releaseDate)
        : this(name, surname, releaseDate)
    {
        Id = id;
    }

}
