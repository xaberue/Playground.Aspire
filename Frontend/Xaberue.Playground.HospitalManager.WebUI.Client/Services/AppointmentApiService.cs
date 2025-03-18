using System.Net.Http.Json;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class AppointmentApiService(HttpClient appointmentsHttpClient) : IAppointmentService
{
    public async Task RegisterAsync(AppointmentRegistrationViewModel registrationViewModel, CancellationToken cancellationToken = default)
    {
        await appointmentsHttpClient.PostAsJsonAsync("/api/appointments", registrationViewModel, cancellationToken);
    }
}
