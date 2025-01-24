using DotnetBarcelona.FilmsManager.WebUI.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();

builder.Services.AddHttpClient<FilmsApiService>(client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<ActorsApiService>(client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();
