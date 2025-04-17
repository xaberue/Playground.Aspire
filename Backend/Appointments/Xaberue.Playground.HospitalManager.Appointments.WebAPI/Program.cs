using ChustaSoft.Auth.ApiKey;
using RabbitMQ.Client;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;
using static Xaberue.Playground.HospitalManager.Appointments.Shared.InfrastructureHelper;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.ConfigureApiKeyAuthentication((token)
    => { return token == builder.Configuration["Auth:ApiKeyToken"]!; });

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

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<AppointmentsGrpcService>();

await app.SetUpRabbitMq();

app.Run();

//TODO: Auth for gRPC API
