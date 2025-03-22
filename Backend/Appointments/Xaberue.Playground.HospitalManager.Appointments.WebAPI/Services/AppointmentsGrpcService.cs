using Grpc.Core;
using MongoDB.Driver;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;

public class AppointmentsGrpcService : Appointments.AppointmentsBase
{

    private readonly IMongoClient _mongoClient;


    public AppointmentsGrpcService(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }


    public override async Task<GetAppointmentsResponse> GetAllToday(GetAllTodayAppointmentsRequest request, ServerCallContext context)
    {
        var db = _mongoClient.GetDatabase("AppointmentsDb");
        var collection = db.GetCollection<Appointment>("Appointments");
        var filter = Builders<Appointment>.Filter.Gt(a => a.Date, DateTime.Today);
        var appointments = (await collection.Find(filter).ToListAsync()).Select(x => x.ToGrpcModel());
        var response = new GetAppointmentsResponse();
        response.Appointments.AddRange(appointments);

        return response;
    }

    public override async Task<GetAppointmentsResponse> GetAllCurrentActive(GetAllCurrentActiveAppointmentsRequest request, ServerCallContext context)
    {
        var db = _mongoClient.GetDatabase("AppointmentsDb");
        var collection = db.GetCollection<Appointment>("Appointments");
        var filter = Builders<Appointment>.Filter.Gt(a => a.Date, DateTime.Today) &
            Builders<Appointment>.Filter.Ne(a => a.Status, AppointmentStatus.Completed);
        var appointments = (await collection.Find(filter).ToListAsync()).Select(x => x.ToGrpcModel());
        var response = new GetAppointmentsResponse();
        response.Appointments.AddRange(appointments);

        return response;
    }

    public override async Task<CreateAppointmentResponse> Create(CreateAppointmentRequest request, ServerCallContext context)
    {
        if (request.DoctorId <= 0 || request.PatientId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "DoctorId and/or PatientId must be set and valid"));

        var appointment = new Appointment(request.PatientId, request.DoctorId, request.Notes);
        var db = _mongoClient.GetDatabase("AppointmentsDb");
        var collection = db.GetCollection<Appointment>("Appointments");

        await collection.InsertOneAsync(appointment);
        var response = new CreateAppointmentResponse { Appointment = appointment.ToGrpcModel() };

        return response;
    }

}
