using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;

public static class RavenDbHelper
{
    public static async Task InitialiseRavenDb(this WebApplication webApplication)
    {
        using (var scope = webApplication.Services.CreateAsyncScope())
        {
            var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

            documentStore.Initialize();
            documentStore.EnsureDatabaseExists();

            await Task.CompletedTask;
        }
    }


    public static void EnsureDatabaseExists(this IDocumentStore store)
    {
        if (string.IsNullOrWhiteSpace(store.Database))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(store.Database));

        try
        {
            store.Maintenance.ForDatabase(store.Database).Send(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            try
            {
                store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
            }
            catch (ConcurrencyException)
            {
                // The database was already created before calling CreateDatabaseOperation
            }

        }
    }

}
