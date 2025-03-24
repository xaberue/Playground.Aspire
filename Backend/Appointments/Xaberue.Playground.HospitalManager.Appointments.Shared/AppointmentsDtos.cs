using System.ComponentModel.DataAnnotations;

namespace Xaberue.Playground.HospitalManager.Appointments.Shared;

public record AppointmentSummariesDto(IEnumerable<AppointmentSummaryDto> Appointments);

public record AppointmentSummaryDto(string Code, string Date, string Box, AppointmentStatus Status);

public record AppointmentRegistrationDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "PatientId must be valid")]
    public int PatientId { get; init; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "DoctorID must be valid")]
    public int DoctorId { get; init; }
    [Required]
    public string Notes { get; init; } = null!;
}