using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Base;

public abstract class RabbitMqBackgroundProcessor<TProcessor> : BackgroundService, IAsyncDisposable
{

    protected string? _queueName;
    protected IChannel? _rabbitMqChannel;

    protected readonly IConnection _rabbitMqConnection;
    protected readonly IAppointmentsRepository _appointmentsRepository;
    protected readonly ILogger<TProcessor> _logger;

    private readonly string _exchangeName;


    public RabbitMqBackgroundProcessor(IConnection rabbitMqConnection, IAppointmentsRepository appointmentsRepository, ILogger<TProcessor> logger, string exchangeName)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _appointmentsRepository = appointmentsRepository;
        _logger = logger;
        _exchangeName = exchangeName;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"RabbitMQ {nameof(TProcessor)} started...");

        _rabbitMqChannel = await _rabbitMqConnection.CreateChannelAsync();
        _queueName = await _rabbitMqChannel.DeclareSubscriptionQueue(_exchangeName);

        var consumer = new AsyncEventingBasicConsumer(_rabbitMqChannel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _rabbitMqChannel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation($"RabbitMQ {nameof(TProcessor)} stopped...");
    }

    protected abstract Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs @event);

    protected async Task PublishAppointmentUpdatedEventAsync(Appointment appointment)
    {
        var appointmentUpdatedEvent = new AppointmentUpdatedEvent(
            appointment.Id.ToString(),
            appointment.Code,
            string.IsNullOrWhiteSpace(appointment.Box) ? "-" : appointment.Box,
            appointment.Status);

        var messageModelSerialized = JsonSerializer.Serialize(appointmentUpdatedEvent);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        await _rabbitMqChannel!.BasicPublishAsync(exchange: "", routingKey: InfrastructureHelper.GetQueueName(InfrastructureHelper.Constants.AppointmentUpdated), body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_rabbitMqChannel is not null)
        {
            await _rabbitMqChannel.CloseAsync();
        }

        _rabbitMqConnection?.Dispose();
    }

}
