using Microsoft.EntityFrameworkCore;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Infrastructure;


public class DoctorsDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }

    public DoctorsDbContext(DbContextOptions<DoctorsDbContext> options)
        : base(options) { }
}
