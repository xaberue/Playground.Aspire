using System.ComponentModel.DataAnnotations;

namespace Xaberue.Playground.HospitalManager.Appointments.Shared;

public record AppointmentDto(string Id, int PatientId, int DoctorId, DateTime Date, string Notes);

public record AppointmentCreationDto
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