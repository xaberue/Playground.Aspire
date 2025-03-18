using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;

public class AppointmentRegisteredProcessor : BackgroundService
{

    private readonly IConnection _rabbitMqConnection;
    private readonly IModel _rabbitMqChannel;
    private readonly IMongoClient _mongoDbClient;
    private readonly ILogger<AppointmentRegisteredProcessor> _logger;


    public AppointmentRegisteredProcessor(IConnection rabbitMqConnection, IMongoClient mongoDbClient, ILogger<AppointmentRegisteredProcessor> logger)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _rabbitMqChannel = _rabbitMqConnection.CreateModel();

        var deadLetterQueueName = $"{AppointmentsConstants.AppointmentRegistered}.DeadLetter";
        _rabbitMqChannel.QueueDeclare(queue: deadLetterQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var mainQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", deadLetterQueueName }
        };

        _rabbitMqChannel.QueueDeclare(queue: AppointmentsConstants.AppointmentRegistered, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArguments);

        _mongoDbClient = mongoDbClient;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ {processor} started...", nameof(AppointmentRegisteredProcessor));

        var consumer = new EventingBasicConsumer(_rabbitMqChannel);
        consumer.Received += OnMessageReceived(stoppingToken);

        _rabbitMqChannel.BasicConsume(queue: AppointmentsConstants.AppointmentRegistered, autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("RabbitMQ {processor} stopped...", nameof(AppointmentRegisteredProcessor));
    }


    private EventHandler<BasicDeliverEventArgs> OnMessageReceived(CancellationToken stoppingToken)
    {
        return async (model, ea) =>
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation requested. Stopping message processing.");
                return;
            }

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                _logger.LogInformation("Received message: {message}", message);
                var creationDto = JsonSerializer.Deserialize<AppointmentRegistrationDto>(message);

                if (creationDto is null)
                    throw new InvalidOperationException("Invalid message content: Unable to deserialize.");

                await ProcessAppointmentRegistration(creationDto);

                _rabbitMqChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {message}", message);
                _rabbitMqChannel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
    }

    private async Task ProcessAppointmentRegistration(AppointmentRegistrationDto creationDto)
    {
        try
        {
            var db = _mongoDbClient.GetDatabase("AppointmentsDb");
            var collection = db.GetCollection<Appointment>("Appointments");
            var appointment = new Appointment(creationDto.PatientId, creationDto.DoctorId, creationDto.Notes);

            await collection.InsertOneAsync(appointment);
            Console.WriteLine("Appointment successfully saved to the database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting appointment into database: {message}", ex.Message);
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
