using Xaberue.Playground.HospitalManager.Patients.Shared;

namespace Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;

public static class PatientExtensions
{
    public static PatientDto ToDtol(this Patient entity)
        => new
        (
            entity.Id,
            entity.Code,
            $"{entity.Name} {entity.Surname}",
            entity.DateOfBirth,
            entity.Nationality
        );

    public static PatientModel ToGrpcModel(this Patient entity)
        => new PatientModel
        {
            Id = entity.Id,
            Code = entity.Code,
            FullName = $"{entity.Name} {entity.Surname}",
            DateOfBirth = entity.DateOfBirth.ToString(),
            Nationality = entity.Nationality
        };
}
