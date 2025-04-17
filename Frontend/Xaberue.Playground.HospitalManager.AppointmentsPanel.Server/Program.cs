using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Configuration;
using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Hubs;
using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Services;
using Xaberue.Playground.HospitalManager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

var grpcEnabled = bool.Parse(builder.Configuration["EnableGrpc"]!);
var appointmentsApiUrl = builder.Configuration.GetConnectionString("AppointmentsApiUrl")
    ?? throw new ArgumentException("AppointmentsApiUrl is mandatory");
var appointmentsApiKey = builder.Configuration["Auth:AppointmentsApiKey"]
    ?? throw new ArgumentException("AppointmentsApiKey is mandatory");
var appointmentsPanelClientUrl = builder.Configuration["AppointmentsPanelClientUrl"]
    ?? throw new ArgumentException("AppointmentsApiUrl is mandatory");

builder.AddServiceDefaults();

var AppointmentsPanelUIPolicyName = "AppointmentsPanelUIPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(AppointmentsPanelUIPolicyName, policy =>
    {
        policy.WithOrigins(appointmentsPanelClientUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddOpenApi();

builder.Services.AddSignalR();

builder.AddRabbitMQClient(connectionName: "HospitalManagerServiceBroker");

if (grpcEnabled)
{
    builder.Services.AddScoped<IAppointmentApiService>(x => new AppointmentGrpcApiClient(appointmentsApiUrl, appointmentsApiKey));
}
else
{
    builder.Services.AddHttpClient(
    HospitalManagerAppointmentsPanelApiConstants.AppointmentsApiClient,
    client =>
    {
        client.BaseAddress = new Uri(appointmentsApiUrl);
    })
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        client.DefaultRequestHeaders.Add("X-ApiKey", builder.Configuration["Auth:AppointmentsApiKey"]!);
    });

    builder.Services.AddScoped<IAppointmentApiService, AppointmentRestApiClient>();
}

builder.Services.AddHostedService<AppointmentUpdatedProcessor>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(AppointmentsPanelUIPolicyName);

app.MapGet("/api/appointments/current", async (IAppointmentApiService appointmentsApiClient) =>
{
    var appointments = await appointmentsApiClient.GetAllCurrentActiveAsync();

    return Results.Ok(appointments);
});

app.MapHub<AppointmentHub>("hub/appointment-updated").RequireCors(AppointmentsPanelUIPolicyName);

app.Run();
