using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;


[Authorize]
public class AppointmentsGrpcService : Appointments.AppointmentsBase
{

    private readonly IMongoClient _mongoClient;
    private readonly AppointmentDailyCodeGeneratorService _appointmentCodeGeneratorService;


    public AppointmentsGrpcService(IMongoClient mongoClient, AppointmentDailyCodeGeneratorService appointmentCodeGeneratorService)
    {
        _mongoClient = mongoClient;
        _appointmentCodeGeneratorService = appointmentCodeGeneratorService;
    }


    public override async Task<AppointmentDetailsCollection> GetAllToday(GetAllTodayAppointmentsRequest request, ServerCallContext context)
    {
        var db = _mongoClient.GetDatabase("AppointmentsDb");
        var collection = db.GetCollection<Appointment>("Appointments");
        var filter = Builders<Appointment>.Filter.Gt(a => a.Date, DateTime.Today);
        var appointments = (await collection.Find(filter).ToListAsync()).Select(x => x.ToDetailGrpcModel());
        var response = new AppointmentDetailsCollection();
        response.Appointments.AddRange(appointments);

        return response;
    }

    public override async Task<AppointmentSummariesCollection> GetAllCurrentActive(GetAllCurrentActiveAppointmentsRequest request, ServerCallContext context)
    {
        var db = _mongoClient.GetDatabase("AppointmentsDb");
        var collection = db.GetCollection<Appointment>("Appointments");
        var filter = Builders<Appointment>.Filter.Gt(a => a.Date, DateTime.Today) &
            Builders<Appointment>.Filter.Ne(a => a.Status, AppointmentStatus.Completed);
        var appointments = (await collection.Find(filter).ToListAsync()).Select(x => x.ToSummaryGrpcModel());
        var response = new AppointmentSummariesCollection();
        response.Appointments.AddRange(appointments);

        return response;
    }

    public override async Task<CreateAppointmentResponse> Create(CreateAppointmentRequest request, ServerCallContext context)
    {
        if (request.DoctorId <= 0 || request.PatientId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "DoctorId and/or PatientId must be set and valid"));

        var generatedCode = await _appointmentCodeGeneratorService.GenerateAsync();
        var appointment = new Appointment(request.PatientId, request.DoctorId, generatedCode, request.Reason);
        var db = _mongoClient.GetDatabase("AppointmentsDb");
        var collection = db.GetCollection<Appointment>("Appointments");

        await collection.InsertOneAsync(appointment);
        var response = new CreateAppointmentResponse { Appointment = appointment.ToSummaryGrpcModel() };

        return response;
    }

}
