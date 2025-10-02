using Microsoft.EntityFrameworkCore;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Infrastructure;


public class DoctorsDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }

    public DoctorsDbContext(DbContextOptions<DoctorsDbContext> options)
        : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(x =>
        {
            x.Property(x => x.Name).HasMaxLength(100).IsRequired();
            x.Property(x => x.Surname).HasMaxLength(100).IsRequired();
            x.Property(x => x.BoxAssigned).HasMaxLength(10).IsRequired();
        });
    }
}
