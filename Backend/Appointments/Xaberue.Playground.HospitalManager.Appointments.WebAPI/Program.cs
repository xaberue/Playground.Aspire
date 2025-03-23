using MongoDB.Driver;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddMongoDBClient(connectionName: "AppointmentsDb");
builder.AddRabbitMQClient(connectionName: "HospitalManagerServiceBroker");

builder.Services.AddSingleton<AppointmentDailyCodeGeneratorService>();

builder.Services.AddHostedService<AppointmentRegisteredProcessor>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGrpcReflectionService();
}

app.UseHttpsRedirection();

app.MapGrpcService<AppointmentsGrpcService>();

app.MapGet("/appointments/today", async (IMongoClient client) =>
{
    var db = client.GetDatabase("AppointmentsDb");
    var collection = db.GetCollection<Appointment>("Appointments");
    var filter = Builders<Appointment>.Filter.Gt(a => a.Date, DateTime.Today);
    var appointments = (await collection.Find(filter).ToListAsync()).Select(x => x.ToSummaryDto());

    return Results.Ok(appointments);
});

app.MapGet("/appointments/current", async (IMongoClient client) =>
{
    var db = client.GetDatabase("AppointmentsDb");
    var collection = db.GetCollection<Appointment>("Appointments");
    var filter = Builders<Appointment>.Filter.Gt(a => a.Date, DateTime.Today) &
        Builders<Appointment>.Filter.Ne(a => a.Status, AppointmentStatus.Completed);
    var appointments = (await collection.Find(filter).ToListAsync()).Select(x => x.ToSummaryDto());

    return Results.Ok(appointments);
});

app.MapPost("/appointment", async (IMongoClient client, AppointmentDailyCodeGeneratorService appointmentCodeGeneratorService, AppointmentRegistrationDto appointmentCreation) =>
{
    var db = client.GetDatabase("AppointmentsDb");
    var collection = db.GetCollection<Appointment>("Appointments");
    var generatedCode = await appointmentCodeGeneratorService.GenerateAsync();
    var appointment = new Appointment(appointmentCreation.PatientId, appointmentCreation.DoctorId, generatedCode, appointmentCreation.Notes);

    await collection.InsertOneAsync(appointment);

    return Results.Created($"/appointments/{appointment.Id}", appointment);
});

app.Run();
