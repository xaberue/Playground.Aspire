using Raven.Client.Documents;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure.RavenDb;

public class AppointmentDailyCountersRavenRepository : IAppointmentDailyCountersRepository
{
    private readonly IDocumentStore _store;

    public AppointmentDailyCountersRavenRepository(IDocumentStore store)
    {
        _store = store;
    }

    public async Task<int> GetForAsync(DateTime date)
    {
        using var session = _store.OpenAsyncSession();
        var today = date.ToString("yyyy-MM-dd");
        var counterDoc = await session.LoadAsync<AppointmentDailyCounter>(today);
        if (counterDoc == null)
        {
            counterDoc = new AppointmentDailyCounter { Id = today, Counter = 1 };
            await session.StoreAsync(counterDoc);
        }
        else
        {
            counterDoc.Counter++;
        }
        await session.SaveChangesAsync();
        return counterDoc.Counter;
    }

}
