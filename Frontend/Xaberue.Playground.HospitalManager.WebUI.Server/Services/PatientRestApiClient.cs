using Xaberue.Playground.HospitalManager.Patients.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Configuration;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class PatientRestApiClient : IPatientApiService
{

    private readonly IHttpClientFactory _httpClientFactory;


    public PatientRestApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public async Task<IEnumerable<PatientGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        var patientsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.PatientsApiClient);
        var patients = await patientsClient.GetFromJsonAsync<PatientDto[]>("/patients") ?? [];

        return patients.Select(x => new PatientGridViewModel(
                x.Id,
                x.Code,
                $"{x.Name} {x.Surname}",
                x.DateOfBirth
           ));
    }

    public async Task<PatientSelectionViewModel?> GetSelectionModelAsync(string code, CancellationToken cancellationToken = default)
    {
        var patientsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.PatientsApiClient);
        var response = await patientsClient.GetFromJsonAsync<PatientDto>($"/patient/{code}");

        return (response != null) ?
            new PatientSelectionViewModel(
                response.Id,
                response.Code,
                $"{response.Name} {response.Surname}")
            : null;
    }

    public async Task<IEnumerable<PatientSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        var patientsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.PatientsApiClient);
        var patients = await patientsClient.GetFromJsonAsync<PatientDto[]>("/patients") ?? [];

        return patients.Select(x => new PatientSelectionViewModel(
                x.Id,
                x.Code,
                $"{x.Name} {x.Surname}"
           ));
    }
}

//TODO: Extract mappers