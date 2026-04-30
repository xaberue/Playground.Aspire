using ChustaSoft.Auth.ApiKey;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure.MongoDb;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure.RavenDb;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.ConfigureApiKeyAuthentication(
    (token) => { return token == builder.Configuration["Auth:ApiKeyToken"]!; });

builder.Services.AddOpenApi();

builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

var noSqlDbType = builder.Configuration["noSqlDb"];

if (noSqlDbType == "MongoDb")
{
    builder.AddMongoDBClient(connectionName: "AppointmentsDb");

    builder.Services.AddSingleton<IAppointmentsRepository, AppointmentsMongoRepository>();
    builder.Services.AddSingleton<IAppointmentDailyCountersRepository, AppointmentDailyCountersMongoRepository>();
}
else if (noSqlDbType == "RavenDb")
{
    builder.AddRavenDBClient(connectionName: "AppointmentsDb");

    builder.Services.AddSingleton<IAppointmentsRepository, AppointmentsRavenRepository>();
    builder.Services.AddSingleton<IAppointmentDailyCountersRepository, AppointmentDailyCountersRavenRepository>();
}
else
    throw new ArgumentException("Invalid NoSQL database type specified in configuration.");

builder.AddRabbitMQClient(connectionName: "RabbitMQ");

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

if (noSqlDbType == "RavenDb")
{
    await app.InitialiseRavenDb();
}

app.Run();
