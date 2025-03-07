using System.ComponentModel.DataAnnotations;

namespace Xaberue.Playground.HospitalManager.Shared;

public record AppointmentCreationViewModel
{
    [Required]
    public PatientSelectionViewModel Patient { get; set; } = null!;
    [Required]
    public DoctorSelectionViewModel Doctor { get; set; } = null!;
    [Required]
    public string Notes { get; set; } = null!;
}