using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Configuration;
using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

var grpcEnabled = bool.Parse(builder.Configuration["EnableGrpc"]!);
var appointmentsApiUrl = builder.Configuration.GetConnectionString("AppointmentsApiUrl")
    ?? throw new ArgumentException("AppointmentsApiUrl is mandatory");
var appointmentsPanelClientUrl = builder.Configuration["AppointmentsPanelClientUrl"]
    ?? throw new ArgumentException("AppointmentsApiUrl is mandatory");

builder.AddServiceDefaults();

builder.Services.AddCors();
builder.Services.AddOpenApi();

builder.AddRabbitMQClient(connectionName: "HospitalManagerServiceBroker");

if (grpcEnabled)
{
    builder.Services.AddScoped<IAppointmentApiService>(x => new AppointmentGrpcApiClient(appointmentsApiUrl));
}
else
{
    builder.Services.AddHttpClient(
    HospitalManagerAppointmentsPanelApiConstants.AppointmentsApiClient,
    client =>
    {
        client.BaseAddress = new Uri(appointmentsApiUrl);
    });

    builder.Services.AddScoped<IAppointmentApiService, AppointmentRestApiClient>();
}


var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(policy => policy.WithOrigins(appointmentsPanelClientUrl).AllowAnyMethod().AllowAnyHeader());

app.MapGet("/api/appointments/current", async (IAppointmentApiService appointmentsApiClient) =>
{
    var appointments = await appointmentsApiClient.GetAllCurrentActiveAsync();

    return Results.Ok(appointments);
});

app.Run();
