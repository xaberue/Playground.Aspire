using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class AppointmentRabbitClient : IAppointmentCommandApiService
{

    private readonly IConnection _connection;
    private readonly IDoctorApiClient _doctorApiClient;


    public AppointmentRabbitClient(IConnection connection, IDoctorApiClient doctorApiClient)
    {
        _connection = connection;
        _doctorApiClient = doctorApiClient;
    }


    public async Task RegisterAsync(AppointmentRegistrationViewModel registrationViewModel, CancellationToken cancellationToken = default)
    {
        using var channel = _connection.CreateModel();

        var messageModel = new AppointmentRegistrationDto
        {
            DoctorId = registrationViewModel.Doctors.First().Id,
            PatientId = registrationViewModel.Patients.First().Id,
            Reason = registrationViewModel.Reason
        };
        var messageModelSerialized = JsonSerializer.Serialize(messageModel);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        channel.BasicPublish(exchange: InfrastructureHelper.GetExchangeName(InfrastructureHelper.Constants.AppointmentRegistered), routingKey: "", basicProperties: null, body: body);

        await Task.CompletedTask;
    }

    public async Task AdmitAsync(AppointmentAdmissionViewModel admissionViewModel, CancellationToken cancellationToken = default)
    {
        using var channel = _connection.CreateModel();

        var doctor = await _doctorApiClient.GetAsync(admissionViewModel.DoctorId, cancellationToken);
        var messageModel = new AppointmentAdmissionDto(admissionViewModel.Id, doctor?.BoxAssigned ?? "Ask at reception desk");
        var messageModelSerialized = JsonSerializer.Serialize(messageModel);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        channel.BasicPublish(exchange: InfrastructureHelper.GetExchangeName(InfrastructureHelper.Constants.AppointmentAdmitted), routingKey: "", basicProperties: null, body: body);

        await Task.CompletedTask;
    }

    public async Task CompleteAsync(AppointmentCompletionViewModel completionViewModel, CancellationToken cancellationToken = default)
    {
        using var channel = _connection.CreateModel();

        var messageModel = new AppointmentCompletionDto(completionViewModel.Id, completionViewModel.Notes);
        var messageModelSerialized = JsonSerializer.Serialize(messageModel);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        channel.BasicPublish(exchange: InfrastructureHelper.GetExchangeName(InfrastructureHelper.Constants.AppointmentCompleted), routingKey: "", basicProperties: null, body: body);

        await Task.CompletedTask;
    }

}
