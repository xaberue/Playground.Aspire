using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Infrastructure;
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

app.Run();
