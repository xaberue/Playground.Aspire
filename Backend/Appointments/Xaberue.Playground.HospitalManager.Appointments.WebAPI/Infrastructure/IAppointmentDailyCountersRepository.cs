namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;

public interface IAppointmentDailyCountersRepository
{
    Task<int> GetForAsync(DateTime date);
}
