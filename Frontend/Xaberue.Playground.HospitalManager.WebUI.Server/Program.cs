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
using Xaberue.Playground.HospitalManager.WebUI.Server.Services;
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

builder.AddRabbitMQClient(connectionName: "HospitalManagerServiceBroker");

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

if (grpcEnabled)
{
    //TODO: Improve that, isolate the gRPC client creation
    builder.Services.AddScoped<IAppointmentQueryApiService>(x => new AppointmentGrpcApiClient(appointmentsApiUrl, doctorsApiUrl, patientsApiUrl));
    builder.Services.AddScoped<IDoctorQueryApiService>(x => new DoctorGrpcApiClient(doctorsApiUrl));
    builder.Services.AddScoped<IPatientQueryApiService>(x => new PatientGrpcApiClient(patientsApiUrl));

    builder.Services.AddScoped<IDoctorApiClient, DoctorGrpcApiClient>(x => new DoctorGrpcApiClient(doctorsApiUrl));
}
else
{
    builder.Services.AddHttpClient(
        HospitalManagerApiConstants.AppointmentsApiClient,
        client =>
        {
            client.BaseAddress = new Uri(appointmentsApiUrl);
        });

    builder.Services.AddHttpClient(
        HospitalManagerApiConstants.PatientsApiClient,
        client =>
        {
            client.BaseAddress = new Uri(patientsApiUrl);
        });

    builder.Services.AddHttpClient(
        HospitalManagerApiConstants.DoctorsApiClient,
        client =>
        {
            client.BaseAddress = new Uri(doctorsApiUrl);
        });

    builder.Services.AddScoped<IAppointmentQueryApiService, AppointmentRestApiClient>();
    builder.Services.AddScoped<IPatientQueryApiService, PatientRestApiClient>();
    builder.Services.AddScoped<IDoctorQueryApiService, DoctorRestApiClient>();

    builder.Services.AddScoped<IDoctorApiClient, DoctorRestApiClient>();
}

builder.Services.AddScoped<IAppointmentCommandApiService, AppointmentRabbitClient>();


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

group.MapGet("/appointments/today", async (IAppointmentQueryApiService appointmentService) =>
{
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
