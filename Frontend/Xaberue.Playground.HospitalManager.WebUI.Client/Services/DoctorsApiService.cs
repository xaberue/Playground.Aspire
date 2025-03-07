using System.Net.Http.Json;
using Xaberue.Playground.HospitalManager.Shared;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class DoctorsApiService(HttpClient doctorsHttpClient)
{
    public async Task<IEnumerable<DoctorGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await doctorsHttpClient.GetFromJsonAsync<IEnumerable<DoctorGridViewModel>>("/api/doctors/grid", cancellationToken);

        return doctors!;
    }

    public async Task<IEnumerable<DoctorSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        try
        {

            var doctors = await doctorsHttpClient.GetFromJsonAsync<IEnumerable<DoctorSelectionViewModel>>("/api/doctors/selection", cancellationToken);

            return doctors!;

        }
        catch (Exception ex)
        {

            throw;
        }
    }

}
