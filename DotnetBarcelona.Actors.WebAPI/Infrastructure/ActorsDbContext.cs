using DotnetBarcelona.Actors.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetBarcelona.Actors.WebAPI.Infrastructure;

public class ActorsDbContext : DbContext
{
    public DbSet<Actor> Actors { get; set; }

    public ActorsDbContext(DbContextOptions<ActorsDbContext> options)
        : base(options) { }
}
