using Raven.Client.Documents;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure.RavenDb;

public class AppointmentsRavenRepository : IAppointmentsRepository
{

    private readonly IDocumentStore _store;


    public AppointmentsRavenRepository(IDocumentStore store)
    {
        _store = store;
    }


    public async Task<IEnumerable<Appointment>> GetAllForToday()
    {
        using var session = _store.OpenAsyncSession();
        var today = DateTime.Today;
        var appointments = await session.Query<AppointmentRavenDbModel>()
            .Where(a => a.Date >= today)
            .ToListAsync();

        return appointments.Select(a => a.ToEntity());
    }

    public async Task<IEnumerable<Appointment>> GetAllCurrentActive()
    {
        using var session = _store.OpenAsyncSession();
        var today = DateTime.Today;
        var appointments = await session.Query<AppointmentRavenDbModel>()
            .Where(a => a.Date > today && a.Status != AppointmentStatus.Completed)
            .ToListAsync();

        return appointments.Select(a => a.ToEntity());
    }

    public async Task InsertAsync(Appointment appointment)
    {
        using var session = _store.OpenAsyncSession();

        var dbModel = appointment.ToDbModel();

        await session.StoreAsync(dbModel);
        await session.SaveChangesAsync();

        appointment.Id = dbModel.Id;
    }

    public async Task<Appointment> UpdateAsync(string id, string box, AppointmentStatus status)
    {
        using var session = _store.OpenAsyncSession();
        var appointment = await session.LoadAsync<AppointmentRavenDbModel>(id);
        if (appointment == null)
            throw new InvalidOperationException($"Appointment with id {id} not found");

        appointment.Box = box;
        appointment.Status = status;

        await session.SaveChangesAsync();

        return appointment.ToEntity();
    }
}
