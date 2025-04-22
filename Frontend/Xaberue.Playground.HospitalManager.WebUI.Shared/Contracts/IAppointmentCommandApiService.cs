using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

public interface IAppointmentCommandApiService
{
    Task RegisterAsync(AppointmentRegistrationViewModel creationViewModel, CancellationToken cancellationToken = default);
    Task AdmitAsync(AppointmentAdmissionViewModel admissionViewModel, CancellationToken cancellationToken = default);
    Task CompleteAsync(AppointmentCompletionViewModel completionViewModel, CancellationToken cancellationToken = default);
}
