using Xaberue.Playground.HospitalManager.Appointments;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Doctors;
using Xaberue.Playground.HospitalManager.Doctors.Shared;
using Xaberue.Playground.HospitalManager.Patients;
using Xaberue.Playground.HospitalManager.Patients.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Appointments;

public static class Mappers
{

    public static AppointmentGridViewModel ToGridModel(this AppointmentDetailDto appointment, DoctorDto doctor, PatientDto patient)
    {
        return new AppointmentGridViewModel(
            appointment.Id,
            appointment.Code,
            appointment.DoctorId,
            $"{doctor.Name} {doctor.Surname}",
            appointment.PatientId,
            $"{patient.Name} {patient.Surname}",
            DateTime.Parse(appointment.Date),
            appointment.Box,
            appointment.Reason,
            appointment.CriticalityLevel,
            appointment.Status
        );
    }

    public static AppointmentGridViewModel ToGridModel(this AppointmentDetailModel appointment, DoctorModel doctor, PatientModel patient)
    {
        return new AppointmentGridViewModel(
            appointment.Id,
            appointment.Code,
            appointment.DoctorId,
            $"{doctor.Name} {doctor.Surname}",
            appointment.PatientId,
            $"{patient.Name} {patient.Surname}",
            DateTime.Parse(appointment.Date),
            appointment.Box,
            appointment.Reason,
            (appointment.Criticality >= 0) ? (CriticalityLevel)appointment.Criticality : null,
            (AppointmentStatus)appointment.Status
        );
    }
}
