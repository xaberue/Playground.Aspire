using Xaberue.Playground.HospitalManager.Appointments;
using Xaberue.Playground.HospitalManager.WebUI.Server.Base;
using AppointmentsGrpc = Xaberue.Playground.HospitalManager.Appointments.Appointments;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Appointments;

public class AppointmentGrpcApiClient : GrpcApiClientBase
{

    public AppointmentGrpcApiClient(string appointmentsApiUrl, string appointmentsApiKey)
        : base(appointmentsApiUrl, appointmentsApiKey)
    { }


    public async Task<AppointmentDetailsCollection> GetAllTodayAsync(CancellationToken cancellationToken = default)
    {
        var appointmentsClient = new AppointmentsGrpc.AppointmentsClient(GrpcChannel);
        
        var appointmentsResponse = await appointmentsClient.GetAllTodayAsync(new GetAllTodayAppointmentsRequest(), headers: Headers, cancellationToken: cancellationToken);

        return appointmentsResponse;
    }

}
