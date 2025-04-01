using System.ComponentModel.DataAnnotations;

namespace Xaberue.Playground.HospitalManager.Appointments.Shared;

public record AppointmentsSummariesDto(IEnumerable<AppointmentSummaryDto> Appointments);
public record AppointmentsDetailsDto(IEnumerable<AppointmentDetailDto> Appointments);

public record AppointmentSummaryDto(string Code, string Date, string Box, AppointmentStatus Status);
public record AppointmentDetailDto(string Id, string Code, int DoctorId, int PatientId, string Date, string Box, string Reason, CriticalityLevel? CriticalityLevel, AppointmentStatus Status);

public record AppointmentRegistrationDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "PatientId must be valid")]
    public int PatientId { get; init; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "DoctorID must be valid")]
    public int DoctorId { get; init; }
    [Required]
    public string Reason { get; init; } = null!;
}

public record AppointmentAdmissionDto(string Id, string Box);

public record AppointmentCompletionDto(string Id, string Notes);