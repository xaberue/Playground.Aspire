using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure.RavenDb;

public static class AppointmentRavenMapper
{
    public static AppointmentRavenDbModel ToDbModel(this Appointment appointment)
    {
        return new AppointmentRavenDbModel
        {
            Id = appointment.Id ?? Guid.NewGuid().ToString(),
            Code = appointment.Code,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            Date = appointment.Date,
            Reason = appointment.Reason,
            Criticality = appointment.Criticality,
            Status = appointment.Status
        };
    }
    public static Appointment ToEntity(this AppointmentRavenDbModel document)
    {
        return new Appointment(
            document.Id,
            document.PatientId,
            document.Code,
            document.DoctorId,
            document.Date,
            document.Reason,
            document.Criticality ?? Shared.CriticalityLevel.Undeterminated,
            document.Status
        );
    }
}
