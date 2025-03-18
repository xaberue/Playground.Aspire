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

app.MapGet("/appointments", async (IMongoClient client) =>
{
    var db = client.GetDatabase("AppointmentsDb");
    var collection = db.GetCollection<Appointment>("Appointments");
    var appointments = (await collection.Find(_ => true).ToListAsync()).Select(x => x.ToDto());

    return Results.Ok(appointments);
});

app.MapPost("/appointment", async (IMongoClient client, AppointmentRegistrationDto appointmentCreation) =>
{
    var db = client.GetDatabase("AppointmentsDb");
    var collection = db.GetCollection<Appointment>("Appointments");
    var appointment = new Appointment(appointmentCreation.PatientId, appointmentCreation.DoctorId, appointmentCreation.Notes);

    await collection.InsertOneAsync(appointment);

    return Results.Created($"/appointments/{appointment.Id}", appointment);
});

app.Run();
