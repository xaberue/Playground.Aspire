using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Base;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;

public class AppointmentAdmittedProcessor : RabbitMqBackgroundProcessor<AppointmentAdmittedProcessor>
{
    public AppointmentAdmittedProcessor(IConnection rabbitMqConnection, IMongoClient mongoDbClient, ILogger<AppointmentAdmittedProcessor> logger)
        : base(rabbitMqConnection, mongoDbClient, logger, InfrastructureHelper.Constants.AppointmentAdmitted)
    { }


    protected override EventHandler<BasicDeliverEventArgs> OnMessageReceived(CancellationToken stoppingToken)
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

                var appointment = await ProcessAppointmentRegistration(admissionDto);

                PublishAppointmentUpdatedEvent(appointment);

                _rabbitMqChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing admission message: {message}", message);
                _rabbitMqChannel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
    }


    private async Task<Appointment> ProcessAppointmentRegistration(AppointmentAdmissionDto registrationDto)
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
                ),
                new FindOneAndUpdateOptions<Appointment>
                {
                    ReturnDocument = ReturnDocument.After
                }
            );

            _logger.LogTrace("Appointment {id} successfully admitted.", registrationDto.Id);

            return appointment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {id} into database: {message}", registrationDto.Id, ex.Message);
            throw;
        }
    }

}
