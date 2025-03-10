using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Xaberue.Playground.HospitalManager.ServiceDefaults;
using Xaberue.Playground.HospitalManager.WebUI.Server.Components;
using Xaberue.Playground.HospitalManager.WebUI.Server.Components.Account;
using Xaberue.Playground.HospitalManager.WebUI.Server.Configuration;
using Xaberue.Playground.HospitalManager.WebUI.Server.Data;
using Xaberue.Playground.HospitalManager.WebUI.Server.Services;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

var grpcEnabled = bool.Parse(builder.Configuration["EnableGrpc"]!);
var patientsApiUrl = builder.Configuration.GetConnectionString("PatientsAPIUrl")
    ?? throw new ArgumentException("PatientsAPIUrl is mandatory");
var doctorsApiUrl = builder.Configuration.GetConnectionString("DoctorsApiUrl")
    ?? throw new ArgumentException("DoctorsApiUrl is mandatory");

builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

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

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

if (grpcEnabled)
{
    builder.Services.AddScoped<IPatientService>(x => new PatientGrpcService(patientsApiUrl));
    builder.Services.AddScoped<IDoctorService>(x => new DoctorGrpcService(doctorsApiUrl));
}
else
{
    builder.Services.AddScoped<IPatientService, PatientRestService>();
    builder.Services.AddScoped<IDoctorService, DoctorRestService>();
}


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

group.MapGet("/doctors/grid", async (IDoctorService doctorService) =>
{
    var data = await doctorService.GetAllGridModelsAsync();

    return data;
})
.WithName("GetAllDoctorGridModels");

group.MapGet("/doctors/selection", async (IDoctorService doctorService) =>
{
    var data = await doctorService.GetAllSelectionModelsAsync();

    return data;
})
.WithName("GetAllDoctorSelectionModels");

group.MapGet("/patients/grid", async (IPatientService patientService) =>
{
    var data = await patientService.GetAllGridModelsAsync();

    return data;
})
.WithName("GetAllPatientGridModels");

group.MapGet("/patients/selection", async (IPatientService patientService) =>
{
    var data = await patientService.GetAllSelectionModelsAsync();

    return data;
})
.WithName("GetAllPatientSelectionModels");

app.MapDefaultEndpoints();

app.UseOutputCache();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Xaberue.Playground.HospitalManager.WebUI.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

app.Run();
