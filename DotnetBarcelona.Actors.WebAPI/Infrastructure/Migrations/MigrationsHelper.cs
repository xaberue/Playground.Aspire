using Microsoft.EntityFrameworkCore;

namespace DotnetBarcelona.Actors.WebAPI.Infrastructure.Migrations;

public static class MigrationsHelper
{
    public static async Task AutomateDbMigrations(this WebApplication webApplication)
    {
        using (var scope = webApplication.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ActorsDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
