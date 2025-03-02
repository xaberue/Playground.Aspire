using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Xaberue.Playground.HospitalManager.WebUI.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();

builder.Services.AddHttpClient<DoctorsApiService>(client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<PatientsApiService>(client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();
