using DotnetBarcelona.Films.Shared;
using DotnetBarcelona.Films.WebAPI.Infrastructure;
using DotnetBarcelona.Films.WebAPI.Infrastructure.Migrations;
using DotnetBarcelona.Films.WebAPI.Models;
using DotnetBarcelona.Films.WebAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddSqlServerDbContext<FilmsDbContext>("FilmsDb");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGrpcReflectionService();
}

app.UseHttpsRedirection();

app.MapGrpcService<FilmsGrpcService>();

app.MapGet("/films", (FilmsDbContext db) =>
{
    var films = db.Films
        .Select(f => f.ToDto())
        .AsAsyncEnumerable();

    return Results.Ok(films);
});

app.MapGet("/film/{filmId}", async (FilmsDbContext db, int filmId) =>
{
    var film = await db.Films.FindAsync(filmId);

    if (film is null)
        return Results.NotFound();

    return Results.Ok(film.ToDto());
});

app.MapPost("/film", async (FilmsDbContext db, FilmCreation filmCreation) =>
{
    var film = new Film(filmCreation.Name, filmCreation.ReleaseDate.Value, filmCreation.Categories, filmCreation.Cast);

    await db.Films.AddAsync(film);
    await db.SaveChangesAsync();

    return Results.Created();
});

app.MapDelete("/film/{filmId}", async (FilmsDbContext db, int filmId) =>
{
    var film = await db.Films.FindAsync(filmId);

    if (film is null)
        return Results.NotFound();

    db.Films.Remove(film);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

await app.AutomateDbMigrations();

app.Run();
