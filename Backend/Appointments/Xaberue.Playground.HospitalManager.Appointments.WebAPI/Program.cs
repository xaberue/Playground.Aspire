using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

builder.AddMongoDBClient(connectionName: "AppointmentsDb");
builder.AddRabbitMQClient(connectionName: "HospitalManagerServiceBroker");

builder.Services.AddSingleton<AppointmentDailyCodeGeneratorService>();

builder.Services.AddHostedService<AppointmentRegisteredProcessor>();
builder.Services.AddHostedService<AppointmentAdmittedProcessor>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGrpcReflectionService();
}

app.UseHttpsRedirection();

app.MapGrpcService<AppointmentsGrpcService>();

app.Run();
