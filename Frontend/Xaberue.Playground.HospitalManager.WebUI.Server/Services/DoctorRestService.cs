using Xaberue.Playground.HospitalManager.Doctors.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Configuration;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class DoctorRestService : IDoctorService
{

    private readonly IHttpClientFactory _httpClientFactory;


    public DoctorRestService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public async Task<IEnumerable<DoctorGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        var doctorsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.DoctorsApiClient);
        var doctors = await doctorsClient.GetFromJsonAsync<DoctorDto[]>("/doctors") ?? [];

        return doctors.Select(x => new DoctorGridViewModel(
               x.Id,
               $"{x.Name} {x.Surname}",
               x.HiringDate
           ));
    }

    public async Task<IEnumerable<DoctorSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        var doctorsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.DoctorsApiClient);
        var doctors = await doctorsClient.GetFromJsonAsync<DoctorDto[]>("/doctors") ?? [];

        return doctors.Select(x => new DoctorSelectionViewModel(
               x.Id,
               $"{x.Name} {x.Surname}"
           ));
    }
}

//TODO: Extract mappers