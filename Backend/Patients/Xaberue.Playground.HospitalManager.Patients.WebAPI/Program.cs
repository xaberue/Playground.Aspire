using Microsoft.EntityFrameworkCore;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;
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

app.MapGet("/patient/{code}", async (PatientsDbContext db, string code) =>
{
    var patient = await db.Patients.FirstOrDefaultAsync(x => x.Code == code);
    if (patient == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(patient.ToDto());
});

app.MapGet("/patients", (PatientsDbContext db) =>
{
    var patients = db.Patients
        .Select(f => f.ToDto())
        .AsAsyncEnumerable();

    return Results.Ok(patients);
});

app.Run();
