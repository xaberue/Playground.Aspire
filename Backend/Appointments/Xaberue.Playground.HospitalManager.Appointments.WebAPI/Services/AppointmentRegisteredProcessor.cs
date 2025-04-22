using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Base;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;

public class AppointmentRegisteredProcessor : RabbitMqBackgroundProcessor<AppointmentRegisteredProcessor>
{

    private readonly AppointmentDailyCodeGeneratorService _appointmentDailyCodeGeneratorService;


    public AppointmentRegisteredProcessor(IConnection rabbitMqConnection, IMongoClient mongoDbClient, ILogger<AppointmentRegisteredProcessor> logger, AppointmentDailyCodeGeneratorService appointmentDailyCodeGeneratorService)
        : base(rabbitMqConnection, mongoDbClient, logger, InfrastructureHelper.Constants.AppointmentRegistered)
    {
        _appointmentDailyCodeGeneratorService = appointmentDailyCodeGeneratorService;
    }


    protected override EventHandler<BasicDeliverEventArgs> OnMessageReceived(CancellationToken stoppingToken)
    {
        return async (model, ea) =>
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation requested. Stopping registration message processing.");
                return;
            }

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                _logger.LogInformation("Received registration message: {message}", message);
                var registrationDto = JsonSerializer.Deserialize<AppointmentRegistrationDto>(message);

                if (registrationDto is null)
                    throw new InvalidOperationException("Invalid registration message content: Unable to deserialize.");

                var appointment = await ProcessAppointmentRegistration(registrationDto);

                PublishAppointmentUpdatedEvent(appointment);

                _rabbitMqChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing registration message: {message}", message);
                _rabbitMqChannel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
    }


    private async Task<Appointment> ProcessAppointmentRegistration(AppointmentRegistrationDto registrationDto)
    {
        try
        {
            var db = _mongoDbClient.GetDatabase("AppointmentsDb");
            var collection = db.GetCollection<Appointment>("Appointments");
            var generatedCode = await _appointmentDailyCodeGeneratorService.GenerateAsync();
            var appointment = new Appointment(registrationDto.PatientId, registrationDto.DoctorId, generatedCode, registrationDto.Reason);

            await collection.InsertOneAsync(appointment);

            _logger.LogTrace("Appointment {id} successfully registered.", appointment.Id);

            return appointment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting appointment into database: {message}", ex.Message);
            throw;
        }
    }

}
