using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Models;

namespace Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Services;

public interface IAppointmentApiService
{
    Task<IEnumerable<AppointmentSummaryViewModel>> GetAllCurrentActiveAsync(CancellationToken cancellationToken = default);
}
