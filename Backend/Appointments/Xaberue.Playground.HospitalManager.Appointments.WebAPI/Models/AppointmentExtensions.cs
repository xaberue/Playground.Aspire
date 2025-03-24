namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

public static class AppointmentExtensions
{
    public static AppointmentSummaryModel ToGrpcModel(this Appointment appointment)
        => new AppointmentSummaryModel
            {
                Code = appointment.Code,
                Date = appointment.Date.ToString(),
                Box = string.IsNullOrWhiteSpace(appointment.Box) ? "-" : appointment.Box,
                Status = (int)appointment.Status
            };

}
