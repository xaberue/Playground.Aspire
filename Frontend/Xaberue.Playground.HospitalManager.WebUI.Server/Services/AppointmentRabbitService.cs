using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class AppointmentRabbitService : IAppointmentService
{

    private readonly IConnection _connection;


    public AppointmentRabbitService(IConnection connection)
    {
        _connection = connection;
    }


    public async Task CreateAsync(AppointmentCreationViewModel creationViewModel, CancellationToken cancellationToken = default)
    {
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: AppointmentsConstants.AppointmentCreated, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var messageModel = new AppointmentCreationDto
        {
            DoctorId = creationViewModel.Doctors.First().Id,
            PatientId = creationViewModel.Patients.First().Id,
            Notes = creationViewModel.Notes
        };
        var messageModelSerialized = JsonSerializer.Serialize(messageModel);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        channel.BasicPublish(exchange: "", routingKey: AppointmentsConstants.AppointmentCreated, basicProperties: null, body: body);

        await Task.CompletedTask;
    }
}
