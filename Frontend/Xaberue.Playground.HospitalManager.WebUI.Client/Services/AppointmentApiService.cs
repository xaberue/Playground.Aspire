using System.Net.Http.Json;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class AppointmentApiService(HttpClient appointmentsHttpClient) : IAppointmentQueryApiService, IAppointmentCommandApiService
{
    public async Task<IEnumerable<AppointmentGridViewModel>> GetAllToday(CancellationToken cancellationToken = default)
    {
        return await appointmentsHttpClient.GetFromJsonAsync<IEnumerable<AppointmentGridViewModel>>("/api/appointments/today", cancellationToken) ?? [];
    }

    public async Task RegisterAsync(AppointmentRegistrationViewModel registrationViewModel, CancellationToken cancellationToken = default)
    {
        await appointmentsHttpClient.PostAsJsonAsync("/api/appointment", registrationViewModel, cancellationToken);
    }

    public async Task AdmitAsync(AppointmentAdmissionViewModel admissionViewModel, CancellationToken cancellationToken = default)
    {
        await appointmentsHttpClient.PutAsJsonAsync("/api/appointment/admit", admissionViewModel, cancellationToken);
    }

    public async Task CompleteAsync(AppointmentCompletionViewModel completionViewModel, CancellationToken cancellationToken = default)
    {
        await appointmentsHttpClient.PutAsJsonAsync("/api/appointment/complete", completionViewModel, cancellationToken);
    }

}
