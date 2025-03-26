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


    public AppointmentRabbitClient(IConnection connection)
    {
        _connection = connection;
    }


    public async Task RegisterAsync(AppointmentRegistrationViewModel registrationViewModel, CancellationToken cancellationToken = default)
    {
        using var channel = _connection.CreateModel();

        var messageModel = new AppointmentRegistrationDto
        {
            DoctorId = registrationViewModel.Doctors.First().Id,
            PatientId = registrationViewModel.Patients.First().Id,
            Notes = registrationViewModel.Notes
        };
        var messageModelSerialized = JsonSerializer.Serialize(messageModel);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        channel.BasicPublish(exchange: "", routingKey: AppointmentsConstants.AppointmentRegistered, basicProperties: null, body: body);

        await Task.CompletedTask;
    }
}
