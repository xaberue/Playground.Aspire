using System.Net.Http.Json;
using Xaberue.Playground.HospitalManager.Shared;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class DoctorsApiService(HttpClient doctorsHttpClient)
{
    public async Task<IEnumerable<DoctorGridViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await doctorsHttpClient.GetFromJsonAsync<IEnumerable<DoctorGridViewModel>>("/api/doctors", cancellationToken);

        return doctors!;
    }

}
