using System.Net.Http.Json;
using Xaberue.Playground.HospitalManager.Shared;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class PatientsApiService(HttpClient patientsHttpClient)
{

    public async Task<IEnumerable<PatientGridViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var patients = await patientsHttpClient.GetFromJsonAsync<IEnumerable<PatientGridViewModel>>("/api/patients", cancellationToken);

        return patients!;
    }
}
