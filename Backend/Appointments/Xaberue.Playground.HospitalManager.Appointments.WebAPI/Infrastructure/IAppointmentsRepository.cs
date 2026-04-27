using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;

public interface IAppointmentsRepository
{
    Task<IEnumerable<Appointment>> GetAllForToday();
    Task<IEnumerable<Appointment>> GetAllCurrentActive();
    Task InsertAsync(Appointment appointment);
    Task<Appointment> UpdateAsync(string id, string box, AppointmentStatus status);
}
