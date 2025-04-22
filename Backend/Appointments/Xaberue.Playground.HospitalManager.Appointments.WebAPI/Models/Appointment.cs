using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

public class Appointment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public string Code { get; init; }
    public int PatientId { get; init; }
    public int DoctorId { get; init; }
    public DateTime Date { get; init; }
    public string? Box { get; set; }
    public string Reason { get; init; }
    public string Notes { get; set; } = string.Empty;
    public CriticalityLevel? Criticality { get; init; }
    public AppointmentStatus Status { get; init; }


    public Appointment(int patientId, int doctorId, string code, DateTime date, string reason, CriticalityLevel? criticality, AppointmentStatus status)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        Code = code;
        Date = date;
        Reason = reason;
        Criticality = criticality;
        Status = status;
    }

    public Appointment(int patientId, int doctorId, string code, string reason)
        : this(patientId, doctorId, code, DateTime.UtcNow, reason, null, AppointmentStatus.Registered)
    { }


    public Appointment(ObjectId id, int patientId, string code, int doctorId, DateTime date, string reason, CriticalityLevel criticality, AppointmentStatus status)
        : this(patientId, doctorId, code, date, reason, criticality, status)
    {
        Id = id;
    }

    public Appointment(int patientId, int doctorId, string code, DateTime date, string reason)
        : this(patientId, doctorId, code, date, reason, null, AppointmentStatus.Admitted)
    { }

}
