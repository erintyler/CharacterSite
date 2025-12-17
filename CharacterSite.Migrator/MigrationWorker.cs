using CharacterSite.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CharacterSite.Migrator;

public class MigrationWorker(IHostApplicationLifetime lifetime, IServiceScopeFactory serviceScopeFactory, ILogger<MigrationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Migrating database schema");
        using var scope = serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CharacterDbContext>();

        try
        {
            await context.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database schema");
            throw;
        }
        finally
        {
            logger.LogInformation("Migrated database");
            lifetime.StopApplication();
        }
    }
}