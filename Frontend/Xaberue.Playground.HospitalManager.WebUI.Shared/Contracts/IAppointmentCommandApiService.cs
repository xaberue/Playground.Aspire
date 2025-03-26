using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

public interface IAppointmentCommandApiService
{
    Task RegisterAsync(AppointmentRegistrationViewModel creationViewModel, CancellationToken cancellationToken = default);
}
