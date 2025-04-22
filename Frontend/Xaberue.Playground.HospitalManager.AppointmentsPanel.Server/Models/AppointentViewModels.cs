using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Models;

public record AppointmentSummaryViewModel(string Id, string Code, DateTime Date, string Box, string Status);

public record AppointmentUpdatedViewModel(string Id, string Code, string Box, string Status);