using DotnetBarcelona.Actors.Shared;
using DotnetBarcelona.Actors.WebAPI.Infrastructure;
using DotnetBarcelona.Actors.WebAPI.Infrastructure.Migrations;
using DotnetBarcelona.Actors.WebAPI.Models;
using DotnetBarcelona.Actors.WebAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();

builder.Services.AddOpenApi();

builder.AddSqlServerDbContext<ActorsDbContext>("ActorsDb");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGrpcService<ActorsGrpcService>();

app.MapGet("/actor/{id}", async (ActorsDbContext db, int id) =>
{
    var actor = await db.Actors.FindAsync(id);
    if (actor == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(actor.ToDto());
});

app.MapGet("/actors", (ActorsDbContext db) =>
{
    var actors = db.Actors
        .Select(a => new ActorDto(a.Id, a.Name, a.DateOfBirth))
        .AsAsyncEnumerable();

    return Results.Ok(actors);
});

app.MapPost("/actors", async (ActorsDbContext db, ActorCreation actorCreation) =>
{
    var actor = new Actor(actorCreation.Name, actorCreation.DateOfBirth, actorCreation.Nationality);

    await db.Actors.AddAsync(actor);
    await db.SaveChangesAsync();

    return Results.Ok(actor.ToDto());
});

await app.AutomateDbMigrations();

app.Run();
