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

    public AppointmentAdmittedProcessor(IConnection rabbitMqConnection, IAppointmentsRepository appointmentsRepository, ILogger<AppointmentAdmittedProcessor> logger)
        : base(rabbitMqConnection, appointmentsRepository, logger, InfrastructureHelper.Constants.AppointmentAdmitted)
    { }


    protected override async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        var body = @event.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        try
        {
            _logger.LogInformation("Received admission message: {message}", message);
            var admissionDto = JsonSerializer.Deserialize<AppointmentAdmissionDto>(message);

            if (admissionDto is null)
                throw new InvalidOperationException("Invalid admission message content: Unable to deserialize.");

            var appointment = await ProcessAppointmentRegistration(admissionDto);

            await PublishAppointmentUpdatedEventAsync(appointment);

            await _rabbitMqChannel!.BasicAckAsync(deliveryTag: @event.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing admission message: {message}", message);
            await _rabbitMqChannel!.BasicNackAsync(deliveryTag: @event.DeliveryTag, multiple: false, requeue: false);
        }
    }


    private async Task<Appointment> ProcessAppointmentRegistration(AppointmentAdmissionDto registrationDto)
    {
        try
        {
            var appointment = await _appointmentsRepository.UpdateAsync(registrationDto.Id, registrationDto.Box, AppointmentStatus.Admitted);

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
