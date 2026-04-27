using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Base;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;

public class AppointmentRegisteredProcessor : RabbitMqBackgroundProcessor<AppointmentRegisteredProcessor>
{

    private readonly AppointmentDailyCodeGeneratorService _appointmentDailyCodeGeneratorService;


    public AppointmentRegisteredProcessor(IConnection rabbitMqConnection, IAppointmentsRepository appointmentsRepository, ILogger<AppointmentRegisteredProcessor> logger, AppointmentDailyCodeGeneratorService appointmentDailyCodeGeneratorService)
        : base(rabbitMqConnection, appointmentsRepository, logger, InfrastructureHelper.Constants.AppointmentRegistered)
    {
        _appointmentDailyCodeGeneratorService = appointmentDailyCodeGeneratorService;
    }


    protected override async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        var body = @event.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        try
        {
            _logger.LogInformation("Received registration message: {message}", message);
            var registrationDto = JsonSerializer.Deserialize<AppointmentRegistrationDto>(message);

            if (registrationDto is null)
                throw new InvalidOperationException("Invalid registration message content: Unable to deserialize.");

            var appointment = await ProcessAppointmentRegistration(registrationDto);

            await PublishAppointmentUpdatedEventAsync(appointment);

            await _rabbitMqChannel!.BasicAckAsync(deliveryTag: @event.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing registration message: {message}", message);
            await _rabbitMqChannel!.BasicNackAsync(deliveryTag: @event.DeliveryTag, multiple: false, requeue: false);
        }
    }


    private async Task<Appointment> ProcessAppointmentRegistration(AppointmentRegistrationDto registrationDto)
    {
        try
        {
            var generatedCode = await _appointmentDailyCodeGeneratorService.GenerateAsync();
            var appointment = new Appointment(registrationDto.PatientId, registrationDto.DoctorId, generatedCode, registrationDto.Reason);

            await _appointmentsRepository.InsertAsync(appointment);
            
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
