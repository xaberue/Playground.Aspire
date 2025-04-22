namespace Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;

public class Patient
{

    public int Id { get; init; }
    public string Code { get; set; }
    public string Name { get; init; } = null!;
    public string Surname { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public string Nationality { get; init; } = null!;


    public Patient(string code, string name, string surname, DateTime dateOfBirth, string nationality)
    {
        Code = code;
        Name = name;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        Nationality = nationality;
    }

    public Patient(int id, string code, string name, string surname, DateTime dateOfBirth, string nationality)
        : this(code, name, surname, dateOfBirth, nationality)
    {
        Id = id;
    }
}
