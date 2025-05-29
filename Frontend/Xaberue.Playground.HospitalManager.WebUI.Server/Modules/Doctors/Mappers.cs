using Xaberue.Playground.HospitalManager.Doctors;
using Xaberue.Playground.HospitalManager.Doctors.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Doctors;

public static class Mappers
{
    public static DoctorGridViewModel ToGridModel(this DoctorDto doctor)
    {
        return new DoctorGridViewModel(
            doctor.Id,
            $"{doctor.Name} {doctor.Surname}",
            doctor.HiringDate
        );
    }

    public static DoctorSelectionViewModel ToSelectionModel(this DoctorDto doctor)
    {
        return new DoctorSelectionViewModel(
            doctor.Id,
            $"{doctor.Name} {doctor.Surname}"
        );
    }

    public static DoctorGridViewModel ToGridModel(this DoctorModel doctor)
    {
        return new DoctorGridViewModel(
            doctor.Id,
            $"{doctor.Name} {doctor.Surname}",
            DateTime.Parse(doctor.HiringDate)
        );
    }

    public static DoctorSelectionViewModel ToSelectionModel(this DoctorModel doctor)
    {
        return new DoctorSelectionViewModel(
            doctor.Id,
            $"{doctor.Name} {doctor.Surname}"
        );
    }
}
