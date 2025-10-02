using Xaberue.Playground.HospitalManager.Patients.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Configuration;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Patients;

public class PatientRestApiClient : IPatientQueryApiService
{

    private readonly IHttpClientFactory _httpClientFactory;


    public PatientRestApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public async Task<IEnumerable<PatientGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        var patientsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.PatientsApiClient);
        var patients = await patientsClient.GetFromJsonAsync<PatientDto[]>("/patients", cancellationToken: cancellationToken) ?? [];

        return patients.Select(x => x.ToGridModel());
    }

    public async Task<PatientSelectionViewModel?> GetSelectionModelAsync(string code, CancellationToken cancellationToken = default)
    {
        var patientsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.PatientsApiClient);
        var response = await patientsClient.GetFromJsonAsync<PatientDto>($"/patient/{code}", cancellationToken: cancellationToken);

        return response != null ?
            response.ToSelectionModel()
            :
            null;
    }

    public async Task<IEnumerable<PatientSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        var patientsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.PatientsApiClient);
        var patients = await patientsClient.GetFromJsonAsync<PatientDto[]>("/patients", cancellationToken: cancellationToken) ?? [];

        return patients.Select(x => x.ToSelectionModel());
    }

}
