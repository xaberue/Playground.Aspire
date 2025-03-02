namespace Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;

public static class PatientExtensions
{
    public static PatientDto ToDto(this Patient entity)
        => new PatientDto
        {
            Id = entity.Id,
            Code = entity.Code,
            FullName = $"{entity.Name} {entity.Surname}",
            DateOfBirth = entity.DateOfBirth.ToString(),
            Nationality = entity.Nationality
        };
}
