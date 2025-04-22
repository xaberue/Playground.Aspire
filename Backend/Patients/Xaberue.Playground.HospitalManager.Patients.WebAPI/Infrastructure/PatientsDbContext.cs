using Microsoft.EntityFrameworkCore;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Patients.WebAPI.Infrastructure;

public class PatientsDbContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }


    public PatientsDbContext(DbContextOptions<PatientsDbContext> options)
        : base(options)
    { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(patient =>
        {
            patient.HasKey(x => x.Id);
            patient.HasIndex(x => x.Code).IsUnique();
            patient.Property(x => x.Name).IsRequired();
            patient.Property(x => x.Surname).IsRequired();
            patient.Property(x => x.DateOfBirth).IsRequired();
            patient.Property(x => x.Nationality).IsRequired();
        });
    }
}
