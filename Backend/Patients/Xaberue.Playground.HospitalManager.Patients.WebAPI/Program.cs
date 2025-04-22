using ChustaSoft.Auth.ApiKey;
using Microsoft.EntityFrameworkCore;
using Xaberue.Playground.HospitalManager.Patients.Shared;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.ConfigureApiKeyAuthentication((token)
    => { return token == builder.Configuration["Auth:ApiKeyToken"]!; });

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

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<PatientsGrpcService>();

app.MapGet("/patient/{code}", async (PatientsDbContext db, string code) =>
{
    var patient = await db.Patients.FirstOrDefaultAsync(x => x.Code == code);
    if (patient == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(patient.ToDto());
}).RequireAuthorization();

app.MapGet("/patients", (HttpContext context, PatientsDbContext db) =>
{
    var idsQueryParam = context.Request.Query["ids"].ToString();
    IAsyncEnumerable<PatientDto>? patients = null;
    if (string.IsNullOrEmpty(idsQueryParam))
    {
        patients = db.Patients
           .Select(f => f.ToDto())
           .AsAsyncEnumerable();
    }
    else
    {
        var idsRequested = idsQueryParam.Split(',').Select(int.Parse).ToList();

        patients = db.Patients
           .Where(x => idsRequested.Contains(x.Id))
           .Select(f => f.ToDto())
           .AsAsyncEnumerable();
    }

    return Results.Ok(patients);
}).RequireAuthorization();

app.Run();

