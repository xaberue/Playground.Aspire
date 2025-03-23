using System.ComponentModel.DataAnnotations;

namespace Xaberue.Playground.HospitalManager.Appointments.Shared;

public record AppointmentSummaryDto(string Code, DateTime Date, string Box, AppointmentStatus Status);

public record AppointmentDetailDto(string Code, DateTime Date, int PatientId, int DoctorId, string Box, string Notes);

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