using RabbitMQ.Client;
using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;

public static class RabbitMqHelper
{
    public static async Task SetUpRabbitMq(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();

        var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
        var channel = connection.CreateModel();

        channel.DeclareExchange(InfrastructureHelper.Constants.AppointmentRegistered);
        channel.DeclareExchange(InfrastructureHelper.Constants.AppointmentAdmitted);
        channel.DeclareExchange(InfrastructureHelper.Constants.AppointmentCompleted);

        channel.DeclareQueue(InfrastructureHelper.Constants.AppointmentUpdated);

        await Task.CompletedTask;
    }
}
