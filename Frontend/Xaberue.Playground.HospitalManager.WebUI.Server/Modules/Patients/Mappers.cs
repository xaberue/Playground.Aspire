using Xaberue.Playground.HospitalManager.Patients;
using Xaberue.Playground.HospitalManager.Patients.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Patients;

public static class Mappers
{
    public static PatientGridViewModel ToGridModel(this PatientDto patient)
    {
        return new PatientGridViewModel(
            patient.Id,
            patient.Code,
            $"{patient.Name} {patient.Surname}",
            patient.DateOfBirth
        );
    }

    public static PatientSelectionViewModel ToSelectionModel(this PatientDto patient)
    {
        return new PatientSelectionViewModel(
            patient.Id,
            patient.Code,
            $"{patient.Name} {patient.Surname}"
        );
    }

    public static PatientGridViewModel ToGridModel(this PatientModel patient)
    {
        return new PatientGridViewModel(
            patient.Id,
            patient.Code,
            $"{patient.Name} {patient.Surname}",
            DateTime.Parse(patient.DateOfBirth)
        );
    }

    public static PatientSelectionViewModel ToSelectionModel(this PatientModel patient)
    {
        return new PatientSelectionViewModel(
            patient.Id,
            patient.Code,
            $"{patient.Name} {patient.Surname}"
        );
    }
}
