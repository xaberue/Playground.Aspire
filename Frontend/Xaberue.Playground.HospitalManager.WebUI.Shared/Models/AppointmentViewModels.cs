using System.ComponentModel.DataAnnotations;
using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

public record AppointmentGridViewModel
    (string Id, string Code, int DoctorId, string DoctorFullName, int PatientId, string PatientFullName, DateTime Date, string? Box, string Reason, CriticalityLevel? Criticality, AppointmentStatus Status);

public record AppointmentRegistrationViewModel
{
    [Required]
    public IEnumerable<PatientSelectionViewModel> Patients { get; set; } = Enumerable.Empty<PatientSelectionViewModel>();
    [Required]
    public IEnumerable<DoctorSelectionViewModel> Doctors { get; set; } = Enumerable.Empty<DoctorSelectionViewModel>();
    [Required]
    public string Reason { get; set; } = null!;
}

public record AppointmentAdmissionViewModel(string Id, int DoctorId);

public record AppointmentCompletionViewModel(string Id, string Notes);