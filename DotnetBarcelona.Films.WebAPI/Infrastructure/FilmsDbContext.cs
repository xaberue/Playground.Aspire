using DotnetBarcelona.Films.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetBarcelona.Films.WebAPI.Infrastructure;


public class FilmsDbContext : DbContext
{
    public DbSet<Film> Films { get; set; }

    public FilmsDbContext(DbContextOptions<FilmsDbContext> options)
        : base(options) { }
}
