using Xaberue.Playground.HospitalManager.Patients.Shared;

namespace Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;

public static class PatientExtensions
{
    public static PatientDto ToDto(this Patient entity)
        => new
        (
            entity.Id,
            entity.Code,
            entity.Name,
            entity.Surname,
            entity.DateOfBirth,
            entity.Nationality
        );

    public static PatientModel ToGrpcModel(this Patient entity)
        => new PatientModel
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Surname = entity.Surname,
            DateOfBirth = entity.DateOfBirth.ToString(),
            Nationality = entity.Nationality
        };
}
