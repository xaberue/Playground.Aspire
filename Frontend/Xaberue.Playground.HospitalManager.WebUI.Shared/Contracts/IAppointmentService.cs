using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

public interface IAppointmentService
{
    Task RegisterAsync(AppointmentRegistrationViewModel creationViewModel, CancellationToken cancellationToken = default);
}
