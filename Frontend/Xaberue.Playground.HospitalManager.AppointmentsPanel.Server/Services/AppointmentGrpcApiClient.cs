using Grpc.Core;
using Grpc.Net.Client;
using RabbitMQ.Client;
using System.Reflection.PortableExecutable;
using Xaberue.Playground.HospitalManager.Appointments;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Models;
using AppointmentsGrpc = Xaberue.Playground.HospitalManager.Appointments.Appointments;

namespace Xaberue.Playground.HospitalManager.AppointmentsPanel.Server.Services;

public class AppointmentGrpcApiClient : IAppointmentApiService
{

    private readonly string _appointmentsApiUrl;
    private readonly string _appointmentsApiKey;


    public AppointmentGrpcApiClient(string appointmentsApiUrl, string appointmentsApiKey)
    {
        _appointmentsApiUrl = appointmentsApiUrl;
        _appointmentsApiKey = appointmentsApiKey;
    }


    public async Task<IEnumerable<AppointmentSummaryViewModel>> GetAllCurrentActiveAsync(CancellationToken cancellationToken = default)
    {
        using var appointmentsChannel = GrpcChannel.ForAddress(_appointmentsApiUrl);

        var headers = new Metadata();
        headers.Add("X-ApiKey", _appointmentsApiKey);
        var appointmentClient = new AppointmentsGrpc.AppointmentsClient(appointmentsChannel);
        var response = await appointmentClient.GetAllCurrentActiveAsync(new GetAllCurrentActiveAppointmentsRequest(), headers: headers, cancellationToken: cancellationToken);

        return response.Appointments.Select(x =>
            new AppointmentSummaryViewModel(
                x.Id,
                x.Code,
                DateTime.Parse(x.Date), x.Box,
                ((AppointmentStatus)x.Status).ToString()
            ));
    }
}
