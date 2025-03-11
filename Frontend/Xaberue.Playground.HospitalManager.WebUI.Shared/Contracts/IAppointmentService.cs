using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

public interface IAppointmentService
{
    Task CreateAsync(AppointmentCreationViewModel creationViewModel, CancellationToken cancellationToken = default);
}
