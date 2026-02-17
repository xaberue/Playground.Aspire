using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Doctors;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Appointments;

public class AppointmentRabbitClient : IAppointmentCommandApiService
{

    private readonly IConnection _rabbitMqConnection;
    private readonly IDoctorApiClient _doctorApiClient;


    public AppointmentRabbitClient(IConnection rabbitMqConnection, IDoctorApiClient doctorApiClient)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _doctorApiClient = doctorApiClient;
    }


    public async Task RegisterAsync(AppointmentRegistrationViewModel registrationViewModel, CancellationToken cancellationToken = default)
    {
        using var channel = await _rabbitMqConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        var messageModel = new AppointmentRegistrationDto
        {
            DoctorId = registrationViewModel.Doctors.First().Id,
            PatientId = registrationViewModel.Patients.First().Id,
            Reason = registrationViewModel.Reason
        };
        var messageModelSerialized = JsonSerializer.Serialize(messageModel);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        await channel.BasicPublishAsync(exchange: InfrastructureHelper.GetExchangeName(InfrastructureHelper.Constants.AppointmentRegistered), routingKey: "", body: body, cancellationToken: cancellationToken);
    }

    public async Task AdmitAsync(AppointmentAdmissionViewModel admissionViewModel, CancellationToken cancellationToken = default)
    {
        using var channel = await _rabbitMqConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        var doctor = await _doctorApiClient.GetAsync(admissionViewModel.DoctorId, cancellationToken);
        var messageModel = new AppointmentAdmissionDto(admissionViewModel.Id, doctor?.BoxAssigned ?? "Ask at reception desk");
        var messageModelSerialized = JsonSerializer.Serialize(messageModel);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        await channel.BasicPublishAsync(exchange: InfrastructureHelper.GetExchangeName(InfrastructureHelper.Constants.AppointmentAdmitted), routingKey: "", body: body, cancellationToken: cancellationToken);
    }

    public async Task CompleteAsync(AppointmentCompletionViewModel completionViewModel, CancellationToken cancellationToken = default)
    {
        using var channel = await _rabbitMqConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        var messageModel = new AppointmentCompletionDto(completionViewModel.Id, completionViewModel.Notes);
        var messageModelSerialized = JsonSerializer.Serialize(messageModel);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        await channel.BasicPublishAsync(exchange: InfrastructureHelper.GetExchangeName(InfrastructureHelper.Constants.AppointmentCompleted), routingKey: "", body: body, cancellationToken: cancellationToken);
    }

}
