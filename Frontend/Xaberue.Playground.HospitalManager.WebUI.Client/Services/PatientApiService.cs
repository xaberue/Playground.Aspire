using System.Net.Http.Json;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class PatientApiService(HttpClient patientsHttpClient) : IPatientService
{

    public async Task<PatientSelectionViewModel?> GetSelectionModelAsync(string code, CancellationToken cancellationToken = default)
    {
        var patient = await patientsHttpClient.GetFromJsonAsync<PatientSelectionViewModel>($"/api/patients/{code}", cancellationToken);

        return patient;
    }

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