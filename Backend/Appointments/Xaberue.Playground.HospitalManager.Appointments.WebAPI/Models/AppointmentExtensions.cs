namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

public static class AppointmentExtensions
{
    public static AppointmentSummaryModel ToSummaryGrpcModel(this Appointment appointment)
        => new()
        {
            Code = appointment.Code,
            Date = appointment.Date.ToString(),
            Box = string.IsNullOrWhiteSpace(appointment.Box) ? "-" : appointment.Box,
            Status = (int)appointment.Status
        };

    public static AppointmentDetailModel ToDetailGrpcModel(this Appointment appointment)
        => new()
        {
            Id = appointment.Id.ToString(),
            Code = appointment.Code,
            DoctorId = appointment.DoctorId,
            PatientId = appointment.PatientId,
            Date = appointment.Date.ToString(),
            Box = string.IsNullOrWhiteSpace(appointment.Box) ? "-" : appointment.Box,
            Notes = appointment.Notes,
            Criticality = (appointment.Criticality is null) ? -1 : (int)appointment.Criticality,
            Status = (int)appointment.Status
        };

}
