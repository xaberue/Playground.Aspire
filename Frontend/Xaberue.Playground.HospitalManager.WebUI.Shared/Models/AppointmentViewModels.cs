using System.ComponentModel.DataAnnotations;
using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

public record AppointmentGridViewModel
{
    public string Id { get; init; }
    public string Code { get; init; }
    public int DoctorId { get; init; }
    public string DoctorFullName { get; init; }
    public int PatientId { get; init; }
    public string PatientFullName { get; init; }
    public DateTime Date { get; init; }
    public string? Box { get; private set; }
    public string Reason { get; init; }
    public CriticalityLevel? Criticality { get; init; }
    public AppointmentStatus Status { get; private set; }


    public AppointmentGridViewModel(
        string id, string code, int doctorId, string doctorFullName,
        int patientId, string patientFullName, DateTime date, string? box,
        string reason, CriticalityLevel? criticality, AppointmentStatus status)
    {
        Id = id;
        Code = code;
        DoctorId = doctorId;
        DoctorFullName = doctorFullName;
        PatientId = patientId;
        PatientFullName = patientFullName;
        Date = date;
        Box = box;
        Reason = reason;
        Criticality = criticality;
        Status = status;
    }


    public void Update(string box, AppointmentStatus status)
    {
        Box = box;
        Status = status;
    }
}

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

public record AppointmentUpdatedViewModel(string Id, string Code, string Box, string Status);