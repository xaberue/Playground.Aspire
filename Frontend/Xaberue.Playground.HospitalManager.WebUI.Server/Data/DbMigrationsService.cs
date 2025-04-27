using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Data;

public class DbMigrationsService : BackgroundService
{

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbMigrationsService> _logger;
    private readonly TimeSpan _startupDelay = TimeSpan.FromSeconds(10);


    public DbMigrationsService(IServiceProvider serviceProvider, ILogger<DbMigrationsService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Migration Service starting... Waiting before applying migrations.");
        await Task.Delay(_startupDelay, stoppingToken);

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _logger.LogInformation("Applying EF Migrations...");
            await dbContext.Database.MigrateAsync(stoppingToken);
            _logger.LogInformation("Migrations applied successfully.");
        }
    }
}