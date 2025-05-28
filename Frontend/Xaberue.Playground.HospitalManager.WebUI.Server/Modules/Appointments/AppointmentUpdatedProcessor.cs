using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Hubs;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;


namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Appointments;

public class AppointmentUpdatedProcessor : BackgroundService
{

    private readonly IConnection _rabbitMqConnection;
    private readonly IModel _rabbitMqChannel;
    private readonly ILogger<AppointmentUpdatedProcessor> _logger;
    private readonly IHubContext<AppointmentHub> _hubContext;


    public AppointmentUpdatedProcessor(IConnection rabbitMqConnection, IHubContext<AppointmentHub> hubContext, ILogger<AppointmentUpdatedProcessor> logger)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _rabbitMqChannel = _rabbitMqConnection.CreateModel();
        _hubContext = hubContext;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ UI {processor} started...", nameof(AppointmentUpdatedProcessor));

        var consumer = new EventingBasicConsumer(_rabbitMqChannel);
        consumer.Received += OnMessageReceived(stoppingToken);

        _rabbitMqChannel.BasicConsume(queue: InfrastructureHelper.GetQueueName(InfrastructureHelper.Constants.AppointmentUpdated), autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("RabbitMQ UI {processor} stopped...", nameof(AppointmentUpdatedProcessor));
    }


    private EventHandler<BasicDeliverEventArgs> OnMessageReceived(CancellationToken stoppingToken)
    {
        return async (model, ea) =>
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation requested. Stopping event message processing.");
                return;
            }

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                _logger.LogInformation("Received event message: {message}", message);
                var appointmentUpdatedEvent = JsonSerializer.Deserialize<AppointmentUpdatedEvent>(message);

                if (appointmentUpdatedEvent is null)
                    throw new InvalidOperationException("Invalid event message content: Unable to deserialize.");

                await ProcessAppointmentRegistration(appointmentUpdatedEvent);

                _rabbitMqChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event message: {message}", message);
                _rabbitMqChannel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
    }

    private async Task ProcessAppointmentRegistration(AppointmentUpdatedEvent appointmentUpdatedEvent)
    {
        try
        {
            var viewModel = new AppointmentUpdatedViewModel(
                appointmentUpdatedEvent.Id,
                appointmentUpdatedEvent.Code,
                appointmentUpdatedEvent.Box,
                appointmentUpdatedEvent.Status.ToString());

            await _hubContext.Clients.All.SendAsync("ReceiveAppointmentUpdated", viewModel);

            _logger.LogTrace("Appointment {id} successfully send to clients.", viewModel.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {id} into database: {message}", appointmentUpdatedEvent.Code, ex.Message);
            throw;
        }
    }


    public override void Dispose()
    {
        _rabbitMqChannel?.Close();
        _rabbitMqConnection?.Close();
        base.Dispose();
    }

}
