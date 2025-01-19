using DotnetBarcelona.FilmsManager.WebUI;
using DotnetBarcelona.FilmsManager.WebUI.Components;
using DotnetBarcelona.FilmsManager.WebUI.Services;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

builder.Services.AddHttpClient<FilmsApiService>(client =>
    {
        client.BaseAddress = new("https+http://webapi");
    });

builder.Services.AddHttpClient<ActorsApiService>(client =>
{
    client.BaseAddress = new("https+http://webapi");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
