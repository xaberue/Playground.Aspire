using Grpc.Net.Client;
using Xaberue.Playground.HospitalManager.Appointments;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Models;
using AppointmentsGrpc = Xaberue.Playground.HospitalManager.Appointments.Appointments;

namespace Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Services;

public class AppointmentGrpcApiClient : IAppointmentApiService
{

    private readonly string _appointmentsApiUrl;


    public AppointmentGrpcApiClient(String appointmentsApiUrl)
    {
        _appointmentsApiUrl = appointmentsApiUrl;
    }


    public async Task<IEnumerable<AppointmentSummaryViewModel>> GetAllCurrentActiveAsync(CancellationToken cancellationToken = default)
    {
        using var appointmentsChannel = GrpcChannel.ForAddress(_appointmentsApiUrl);

        var appointmentClient = new AppointmentsGrpc.AppointmentsClient(appointmentsChannel);
        var response = await appointmentClient.GetAllCurrentActiveAsync(new GetAllCurrentActiveAppointmentsRequest());

        return response.Appointments.Select(x =>
            new AppointmentSummaryViewModel(
                x.Code,
                DateTime.Parse(x.Date), x.Box,
                ((AppointmentStatus)x.Status).ToString()
            ));
    }
}
