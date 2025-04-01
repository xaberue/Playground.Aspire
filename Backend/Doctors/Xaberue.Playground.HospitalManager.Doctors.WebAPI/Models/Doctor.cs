namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

public class Doctor
{

    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Surname { get; init; } = null!;
    public string BoxAssigned { get; set; }
    public DateTime HiringDate { get; init; }


    public Doctor(string name, string surname, string boxAssigned, DateTime hiringDate)
    {
        Name = name;
        Surname = surname;
        BoxAssigned = boxAssigned;
        HiringDate = hiringDate;
    }

    public Doctor(int id, string name, string surname, string boxAssigned, DateTime releaseDate)
        : this(name, surname, boxAssigned, releaseDate)
    {
        Id = id;
    }

}
