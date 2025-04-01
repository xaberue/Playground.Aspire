using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;

public class AppointmentAdmittedProcessor : BackgroundService
{

    private readonly IConnection _rabbitMqConnection;
    private readonly IModel _rabbitMqChannel;
    private readonly IMongoClient _mongoDbClient;
    private readonly ILogger<AppointmentAdmittedProcessor> _logger;


    public AppointmentAdmittedProcessor(IConnection rabbitMqConnection, IMongoClient mongoDbClient, ILogger<AppointmentAdmittedProcessor> logger)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _rabbitMqChannel = _rabbitMqConnection.CreateModel();

        var deadLetterQueueName = $"{AppointmentsConstants.AppointmentAdmitted}.DeadLetter";
        _rabbitMqChannel.QueueDeclare(queue: deadLetterQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var mainQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", deadLetterQueueName }
        };

        _rabbitMqChannel.QueueDeclare(queue: AppointmentsConstants.AppointmentAdmitted, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArguments);

        _mongoDbClient = mongoDbClient;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ {processor} started...", nameof(AppointmentAdmittedProcessor));

        var consumer = new EventingBasicConsumer(_rabbitMqChannel);
        consumer.Received += OnMessageReceived(stoppingToken);

        _rabbitMqChannel.BasicConsume(queue: AppointmentsConstants.AppointmentAdmitted, autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("RabbitMQ {processor} stopped...", nameof(AppointmentAdmittedProcessor));
    }


    private EventHandler<BasicDeliverEventArgs> OnMessageReceived(CancellationToken stoppingToken)
    {
        return async (model, ea) =>
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation requested. Stopping admission message processing.");
                return;
            }

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                _logger.LogInformation("Received admission message: {message}", message);
                var admissionDto = JsonSerializer.Deserialize<AppointmentAdmissionDto>(message);

                if (admissionDto is null)
                    throw new InvalidOperationException("Invalid admission message content: Unable to deserialize.");

                await ProcessAppointmentRegistration(admissionDto);

                _rabbitMqChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing admission message: {message}", message);
                _rabbitMqChannel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
    }

    private async Task ProcessAppointmentRegistration(AppointmentAdmissionDto registrationDto)
    {
        try
        {
            var db = _mongoDbClient.GetDatabase("AppointmentsDb");
            var collection = db.GetCollection<Appointment>("Appointments");
            var appointment = await collection.FindOneAndUpdateAsync<Appointment>(
                Builders<Appointment>.Filter.Eq(a => a.Id, ObjectId.Parse(registrationDto.Id)),
                Builders<Appointment>.Update.Combine(
                    Builders<Appointment>.Update.Set(a => a.Status, AppointmentStatus.Admitted),
                    Builders<Appointment>.Update.Set(a => a.Box, registrationDto.Box)
                )
            );

            _logger.LogTrace("Appointment {id} successfully admitted.", registrationDto.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {id} into database: {message}", registrationDto.Id, ex.Message);
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
