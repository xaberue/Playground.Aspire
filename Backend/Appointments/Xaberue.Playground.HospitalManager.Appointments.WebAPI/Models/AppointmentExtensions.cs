using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

public static class AppointmentExtensions
{
    public static AppointmentSummaryDto ToSummaryDto(this Appointment appointment)
        => new(appointment.Id.ToString(), appointment.Date, string.IsNullOrWhiteSpace(appointment.Box) ? "-" : appointment.Box, appointment.Status);

    public static AppointmentDetailDto ToDetailDto(this Appointment appointment)
        => new(appointment.Id.ToString(), appointment.PatientId, appointment.DoctorId, string.IsNullOrWhiteSpace(appointment.Box) ? "-" : appointment.Box, appointment.Date, appointment.Notes);

    public static AppointmentSummaryModel ToGrpcModel(this Appointment appointment)
        => new AppointmentSummaryModel
            {
                Id = appointment.Id.ToString(),
                Date = appointment.Date.ToString(),
                Box = string.IsNullOrWhiteSpace(appointment.Box) ? "-" : appointment.Box,
                Status = (int)appointment.Status
            };

}
