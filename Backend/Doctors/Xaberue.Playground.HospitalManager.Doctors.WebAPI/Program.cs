using Microsoft.EntityFrameworkCore;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddSqlServerDbContext<DoctorsDbContext>("DoctorsDb");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGrpcReflectionService();

    await app.AutomateDbMigrations();
}

app.UseHttpsRedirection();

app.MapGrpcService<DoctorsGrpcService>();

app.MapGet("/doctors", (DoctorsDbContext db) =>
{
    var doctors = db.Doctors.Select(f => f.ToDto()).AsAsyncEnumerable();

    return Results.Ok(doctors);
});

app.MapGet("/doctor/{id}", async (DoctorsDbContext db, int id) =>
{
    var doctor = await db.Doctors.FindAsync(id);

    return (doctor is null)
        ? Results.NotFound()
        :
        Results.Ok(doctor.ToDto());
});

app.Run();