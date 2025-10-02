using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;
using System.Threading.Channels;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Base;

public abstract class RabbitMqBackgroundProcessor<TProcessor> : BackgroundService
{

    protected readonly string _queueName;
    protected readonly IConnection _rabbitMqConnection;
    protected readonly IModel _rabbitMqChannel;
    protected readonly IMongoClient _mongoDbClient;
    protected readonly ILogger<TProcessor> _logger;


    public RabbitMqBackgroundProcessor(IConnection rabbitMqConnection, IMongoClient mongoDbClient, ILogger<TProcessor> logger, string exchangeName)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _rabbitMqChannel = _rabbitMqConnection.CreateModel();
        _queueName = _rabbitMqChannel.DeclareSubscriptionQueue(exchangeName);
        _mongoDbClient = mongoDbClient;
        _logger = logger;
    }


    public override void Dispose()
    {
        _rabbitMqChannel?.Close();
        _rabbitMqConnection?.Close();
        base.Dispose();
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"RabbitMQ {nameof(TProcessor)} started...");

        var consumer = new EventingBasicConsumer(_rabbitMqChannel);
        consumer.Received += OnMessageReceived(stoppingToken);

        _rabbitMqChannel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation($"RabbitMQ {nameof(TProcessor)} stopped...");
    }

    protected abstract EventHandler<BasicDeliverEventArgs> OnMessageReceived(CancellationToken stoppingToken);

    protected void PublishAppointmentUpdatedEvent(Appointment appointment)
    {
        var appointmentUpdatedEvent = new AppointmentUpdatedEvent(
            appointment.Id.ToString(),
            appointment.Code,
            string.IsNullOrWhiteSpace(appointment.Box) ? "-" : appointment.Box,
            appointment.Status);

        var messageModelSerialized = JsonSerializer.Serialize(appointmentUpdatedEvent);
        var body = Encoding.UTF8.GetBytes(messageModelSerialized);

        _rabbitMqChannel.BasicPublish(exchange: "", routingKey: InfrastructureHelper.GetQueueName(InfrastructureHelper.Constants.AppointmentUpdated), basicProperties: null, body: body);
    }

}
