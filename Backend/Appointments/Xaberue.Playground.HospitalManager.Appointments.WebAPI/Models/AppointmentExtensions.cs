using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

public static class AppointmentExtensions
{

    public static AppointmentDto ToDto(this Appointment appointment)
        => new(appointment.Id.ToString(), appointment.PatientId, appointment.DoctorId, appointment.Date, appointment.Notes);

    public static AppointmentModel ToGrpcModel(this Appointment appointment)
        => new AppointmentModel
        {
            Id = appointment.Id.ToString(),
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            Date = appointment.Date.ToString(),
            Notes = appointment.Notes
        };

}
