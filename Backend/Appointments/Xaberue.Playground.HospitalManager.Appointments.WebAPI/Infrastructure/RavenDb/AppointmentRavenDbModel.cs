using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure.RavenDb;

public class AppointmentRavenDbModel
{

    public string Id { get; init; }
    public string Code { get; init; } = null!;
    public int PatientId { get; init; }
    public int DoctorId { get; init; }
    public DateTime Date { get; init; }
    public string? Box { get; set; }
    public string Reason { get; init; } = null!;
    public string Notes { get; set; } = string.Empty;
    public CriticalityLevel? Criticality { get; init; }
    public AppointmentStatus Status { get; set; }

}
