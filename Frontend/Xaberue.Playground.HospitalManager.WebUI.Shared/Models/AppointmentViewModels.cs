using System.ComponentModel.DataAnnotations;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

public record AppointmentRegistrationViewModel
{
    [Required]
    public IEnumerable<PatientSelectionViewModel> Patients { get; set; } = Enumerable.Empty< PatientSelectionViewModel>();
    [Required]
    public IEnumerable<DoctorSelectionViewModel> Doctors { get; set; } = Enumerable.Empty<DoctorSelectionViewModel>();
    [Required]
    public string Notes { get; set; } = null!;
}