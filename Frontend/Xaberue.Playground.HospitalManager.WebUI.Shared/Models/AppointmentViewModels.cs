using System.ComponentModel.DataAnnotations;
using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

public record AppointmentGridViewModel
    (string Id, string Code, int doctorId, string DoctorFullName, int patientId, string PatientFullName, DateTime Date, string? Box, string Notes, CriticalityLevel? Criticality, AppointmentStatus Status);

public record AppointmentRegistrationViewModel
{
    [Required]
    public IEnumerable<PatientSelectionViewModel> Patients { get; set; } = Enumerable.Empty< PatientSelectionViewModel>();
    [Required]
    public IEnumerable<DoctorSelectionViewModel> Doctors { get; set; } = Enumerable.Empty<DoctorSelectionViewModel>();
    [Required]
    public string Notes { get; set; } = null!;
}