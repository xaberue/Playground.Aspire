namespace Xaberue.Playground.HospitalManager.Patients.Shared;

public record PatientDto(int Id, string Code, string Name, string Surname, DateTime DateOfBirth, string Nationality);
