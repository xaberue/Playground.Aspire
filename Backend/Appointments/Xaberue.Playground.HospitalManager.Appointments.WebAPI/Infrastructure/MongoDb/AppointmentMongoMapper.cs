using MongoDB.Bson;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure.MongoDb;

public static class AppointmentMongoMapper
{
    public static AppointmentMongoDbModel ToDbModel(this Appointment appointment)
    {
        return new AppointmentMongoDbModel
        {
            Id = string.IsNullOrEmpty(appointment.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(appointment.Id),
            Code = appointment.Code,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            Date = appointment.Date,
            Reason = appointment.Reason,
            Criticality = appointment.Criticality,
            Status = appointment.Status
        };
    }
    public static Appointment ToEntity(this AppointmentMongoDbModel document)
    {
        return new Appointment(
            document.Id.ToString(),
            document.PatientId,
            document.Code,
            document.DoctorId,
            document.Date,
            document.Reason,
            document.Criticality ?? Shared.CriticalityLevel.Undeterminated,
            document.Status
        );
    }
}
