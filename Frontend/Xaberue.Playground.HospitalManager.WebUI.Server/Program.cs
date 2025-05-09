using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.FluentUI.AspNetCore.Components;
using Xaberue.Playground.HospitalManager.ServiceDefaults;
using Xaberue.Playground.HospitalManager.WebUI.Server.Components;
using Xaberue.Playground.HospitalManager.WebUI.Server.Components.Account;
using Xaberue.Playground.HospitalManager.WebUI.Server.Configuration;
using Xaberue.Playground.HospitalManager.WebUI.Server.Data;
using Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Appointments;
using Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Doctors;
using Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Patients;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

var grpcEnabled = bool.Parse(builder.Configuration["EnableGrpc"]!);

var appointmentsApiUrl = builder.Configuration.GetConnectionString("AppointmentsApiUrl")
    ?? throw new ArgumentException("PatientsAPIUrl is mandatory");
var patientsApiUrl = builder.Configuration.GetConnectionString("PatientsAPIUrl")
    ?? throw new ArgumentException("PatientsAPIUrl is mandatory");
var doctorsApiUrl = builder.Configuration.GetConnectionString("DoctorsApiUrl")
    ?? throw new ArgumentException("DoctorsApiUrl is mandatory");

var appointmentsApiKey = builder.Configuration["Auth:AppointmentsApiKey"]
    ?? throw new ArgumentException("AppointmentsApiKey is mandatory");
var patientsApiKey = builder.Configuration["Auth:PatientsApiKey"]
    ?? throw new ArgumentException("PatientsApiKey is mandatory");
var doctorsApiKey = builder.Configuration["Auth:DoctorsApiKey"]
    ?? throw new ArgumentException("DoctorsApiKey is mandatory");

builder.AddServiceDefaults();

builder.AddRedisOutputCache("cache");
builder.AddRedisDistributedCache("cache");
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
        Expiration = TimeSpan.FromMinutes(2)
    };
});

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();
builder.Services.AddFluentUIComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.AddSqlServerDbContext<ApplicationDbContext>("IdentityDb");
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.AddRabbitMQClient(connectionName: "RabbitMQ");

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

if (grpcEnabled)
{
    builder.Services.AddScoped(x => new AppointmentGrpcApiClient(appointmentsApiUrl, appointmentsApiKey));
    builder.Services.AddScoped(x => new DoctorGrpcApiClient(doctorsApiUrl, doctorsApiKey));
    builder.Services.AddScoped(x => new PatientGrpcApiClient(patientsApiUrl, patientsApiKey));

    builder.Services.AddScoped<IAppointmentQueryApiService, AppointmentGrpcApiService>();
    builder.Services.AddScoped<IDoctorQueryApiService, DoctorGrpcApiService>();
    builder.Services.AddScoped<IPatientQueryApiService, PatientGrpcApiService>();

    builder.Services.AddScoped<IDoctorApiClient, DoctorGrpcApiService>();
}
else
{
    builder.Services.AddHttpClient(
        HospitalManagerApiConstants.AppointmentsApiClient,
        client =>
        {
            client.BaseAddress = new Uri(appointmentsApiUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("HospitalManager-WebUI");
        })
        .ConfigureHttpClient((serviceProvider, client) =>
        {
            client.DefaultRequestHeaders.Add("X-ApiKey", appointmentsApiKey);
        });

    builder.Services.AddHttpClient(
        HospitalManagerApiConstants.PatientsApiClient,
        client =>
        {
            client.BaseAddress = new Uri(patientsApiUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("HospitalManager-WebUI");
        })
        .ConfigureHttpClient((serviceProvider, client) =>
        {
            client.DefaultRequestHeaders.Add("X-ApiKey", patientsApiKey);
        });

    builder.Services.AddHttpClient(
        HospitalManagerApiConstants.DoctorsApiClient,
        client =>
        {
            client.BaseAddress = new Uri(doctorsApiUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("HospitalManager-WebUI");
        })
        .ConfigureHttpClient((serviceProvider, client) =>
        {
            client.DefaultRequestHeaders.Add("X-ApiKey", doctorsApiKey);
        });

    builder.Services.AddScoped<IAppointmentQueryApiService, AppointmentRestApiClient>();
    builder.Services.AddScoped<IPatientQueryApiService, PatientRestApiClient>();
    builder.Services.AddScoped<IDoctorQueryApiService, DoctorRestApiClient>();

    builder.Services.AddScoped<IDoctorApiClient, DoctorRestApiClient>();
}

builder.Services.AddScoped<IAppointmentCommandApiService, AppointmentRabbitClient>();

builder.Services.AddHostedService<DbMigrationsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

var group = app.MapGroup("/api");

group.MapGet("/doctors/grid", async (HybridCache cache, IDoctorQueryApiService doctorService, CancellationToken cancellationToken) =>
{
    var data = await cache.GetOrCreateAsync("DOCTORS-GRID", async entry =>
    {
        return await doctorService.GetAllGridModelsAsync(entry);
    },
    tags: ["DOCTORS-GRID"],
    cancellationToken: cancellationToken
    );

    return data;
})
.WithName("GetAllDoctorGridModels");

group.MapGet("/doctors/selection", async (IDoctorQueryApiService doctorService) =>
{
    var data = await doctorService.GetAllSelectionModelsAsync();

    return data;
})
.WithName("GetAllDoctorSelectionModels");

group.MapGet("/patients/grid", async (IPatientQueryApiService patientService) =>
{
    var data = await patientService.GetAllGridModelsAsync();

    return data;
})
.WithName("GetAllPatientGridModels");

group.MapGet("/patients/{code}", async (string code, IPatientQueryApiService patientService) =>
{
    var data = await patientService.GetSelectionModelAsync(code);

    return data;
})
.WithName("GetPatientSelectionModelByCode");

group.MapGet("/patients/selection", async (IPatientQueryApiService patientService) =>
{
    var data = await patientService.GetAllSelectionModelsAsync();

    return data;
})
.WithName("GetAllPatientSelectionModels");

group.MapGet("/appointments/today", async (IAppointmentQueryApiService appointmentService, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("GetAllTodayAppointments");
    logger.LogInformation("GetAllTodayAppointments called");

    var data = await appointmentService.GetAllToday();

    return data;
})
.WithName("GetAllTodayAppointments");

group.MapPost("/appointment", async (IAppointmentCommandApiService appointmentService, AppointmentRegistrationViewModel creationModel, CancellationToken cancellationToken) =>
{
    await appointmentService.RegisterAsync(creationModel, cancellationToken);

    return Results.Accepted();
})
.WithName("RegisterAppointment");

group.MapPut("/appointment/admit", async (IAppointmentCommandApiService appointmentService, AppointmentAdmissionViewModel admissionModel, CancellationToken cancellationToken) =>
{
    await appointmentService.AdmitAsync(admissionModel, cancellationToken);

    return Results.Accepted();
})
.WithName("AdmitAppointment");

group.MapPut("/appointment/complete", async (IAppointmentCommandApiService appointmentService, AppointmentCompletionViewModel completionModel, CancellationToken cancellationToken) =>
{
    await appointmentService.CompleteAsync(completionModel, cancellationToken);

    return Results.Accepted();
})
.WithName("CompleteAppointment");

//TODO: Extract mappings

//TODO: Move Endpoints to a RouteGroupeBuilder extension method

app.MapDefaultEndpoints();

app.UseOutputCache();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Xaberue.Playground.HospitalManager.WebUI.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

app.Run();
