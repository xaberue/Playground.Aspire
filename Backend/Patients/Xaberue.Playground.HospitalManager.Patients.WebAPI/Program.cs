using Xaberue.Playground.HospitalManager.Patients.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddOpenApi();

builder.AddSqlServerDbContext<PatientsDbContext>("PatientsDb");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGrpcReflectionService();

    await app.AutomateDbMigrations();
}

app.UseHttpsRedirection();

app.MapGrpcService<PatientsGrpcService>();

app.Run();
