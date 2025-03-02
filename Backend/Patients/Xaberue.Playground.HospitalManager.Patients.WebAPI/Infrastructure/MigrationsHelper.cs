using Microsoft.EntityFrameworkCore;

namespace Xaberue.Playground.HospitalManager.Patients.WebAPI.Infrastructure;

public static class MigrationsHelper
{
    public static async Task AutomateDbMigrations(this WebApplication webApplication)
    {
        using (var scope = webApplication.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<PatientsDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
