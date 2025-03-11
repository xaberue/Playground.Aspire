using System.Net.Http.Json;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class DoctorApiService(HttpClient doctorsHttpClient) : IDoctorService
{
    public async Task<IEnumerable<DoctorGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await doctorsHttpClient.GetFromJsonAsync<IEnumerable<DoctorGridViewModel>>("/api/doctors/grid", cancellationToken);

        return doctors!;
    }

    public async Task<IEnumerable<DoctorSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await doctorsHttpClient.GetFromJsonAsync<IEnumerable<DoctorSelectionViewModel>>("/api/doctors/selection", cancellationToken);

        return doctors!;
    }

}
