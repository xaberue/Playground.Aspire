using Microsoft.EntityFrameworkCore;

namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Infrastructure;

public static class MigrationsHelper
{
    public static async Task AutomateDbMigrations(this WebApplication webApplication)
    {
        using (var scope = webApplication.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DoctorsDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
