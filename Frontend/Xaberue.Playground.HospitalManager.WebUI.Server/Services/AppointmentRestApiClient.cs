using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class AppointmentRestApiClient : IAppointmentQueryApiService
{

    private readonly IHttpClientFactory _httpClientFactory;


    public AppointmentRestApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public Task<IEnumerable<AppointmentGridViewModel>> GetAllToday(CancellationToken cancellationToken = default)
    {



        throw new NotImplementedException();
    }
}
