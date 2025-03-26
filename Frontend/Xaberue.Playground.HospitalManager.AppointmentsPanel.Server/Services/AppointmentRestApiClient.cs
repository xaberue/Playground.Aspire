using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Configuration;
using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Models;

namespace Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Services;

public class AppointmentRestApiClient : IAppointmentApiService
{

    private readonly IHttpClientFactory _httpClientFactory;


    public AppointmentRestApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public async Task<IEnumerable<AppointmentSummaryViewModel>> GetAllCurrentActiveAsync(CancellationToken cancellationToken = default)
    {
        var appointmentsClient = _httpClientFactory.CreateClient(HospitalManagerAppointmentsPanelApiConstants.AppointmentsApiClient);
        var appointments = await appointmentsClient.GetFromJsonAsync<AppointmentsSummariesDto>("/appointments/current");

        return (appointments is null) ? [] : appointments.Appointments.Select(x =>
            new AppointmentSummaryViewModel(
                x.Code,
                DateTime.Parse(x.Date),
                x.Box,
                x.Status.ToString()
           ));
    }
}
