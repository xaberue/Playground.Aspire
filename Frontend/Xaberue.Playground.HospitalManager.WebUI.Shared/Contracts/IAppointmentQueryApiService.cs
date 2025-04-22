using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

public interface IAppointmentQueryApiService
{
    Task<IEnumerable<AppointmentGridViewModel>> GetAllToday(CancellationToken cancellationToken = default);
}
