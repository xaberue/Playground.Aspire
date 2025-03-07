using System.Net.Http.Json;
using Xaberue.Playground.HospitalManager.Shared;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class PatientsApiService(HttpClient patientsHttpClient)
{

    public async Task<IEnumerable<PatientGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        var patients = await patientsHttpClient.GetFromJsonAsync<IEnumerable<PatientGridViewModel>>("/api/patients/grid", cancellationToken);

        return patients!;
    }

    public async Task<IEnumerable<PatientSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        var patients = await patientsHttpClient.GetFromJsonAsync<IEnumerable<PatientSelectionViewModel>>("/api/patients/selection", cancellationToken);

        return patients!;
    }
}
