namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

public class Doctor
{

    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime HiringDate { get; init; }


    public Doctor(string name, DateTime hiringDate)
    {
        Name = name;
        HiringDate = hiringDate;
    }

    public Doctor(int id, string name, DateTime releaseDate)
        : this(name, releaseDate)
    {
        Id = id;
    }

}
