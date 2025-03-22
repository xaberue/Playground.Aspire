using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

public class Appointment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    
    public int PatientId { get; init; }
    public int DoctorId { get; init; }
    public DateTime Date { get; init; }
    public string? Box { get; set; }
    public string Notes { get; init; }
    public CriticalityLevel? Criticality { get; init; }
    public AppointmentStatus Status { get; init; }


    public Appointment(int patientId, int doctorId, DateTime date, string notes, CriticalityLevel? criticality, AppointmentStatus status)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        Date = date;
        Notes = notes;
        Criticality = criticality;
        Status = status;
    }

    public Appointment(int patientId, int doctorId, string notes)
        : this(patientId, doctorId, DateTime.UtcNow, notes, null, AppointmentStatus.Admitted)
    { }


    public Appointment(ObjectId id, int patientId, int doctorId, DateTime date, string notes, CriticalityLevel criticality, AppointmentStatus status)
        : this(patientId, doctorId, date, notes, criticality, status)
    {
        Id = id;
    }

    public Appointment(int patientId, int doctorId, DateTime date, string notes)
        : this(patientId, doctorId, date, notes, null, AppointmentStatus.Admitted)
    { }

}
