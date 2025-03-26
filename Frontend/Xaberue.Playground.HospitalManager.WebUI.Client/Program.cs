using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Xaberue.Playground.HospitalManager.WebUI.Client.Services;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddHttpClient<IAppointmentQueryApiService, AppointmentApiService>(client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<IAppointmentCommandApiService, AppointmentApiService>(client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<IDoctorQueryApiService, DoctorApiService>(client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<IPatientQueryApiService, PatientApiService>(client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();
