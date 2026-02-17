using RabbitMQ.Client;
using Xaberue.Playground.HospitalManager.Appointments.Shared;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;

public static class RabbitMqHelper
{
    public static async Task SetUpRabbitMq(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();

        var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
        var channel = await connection.CreateChannelAsync();

        await channel.DeclareExchangeAsync(InfrastructureHelper.Constants.AppointmentRegistered);
        await channel.DeclareExchangeAsync(InfrastructureHelper.Constants.AppointmentAdmitted);
        await channel.DeclareExchangeAsync(InfrastructureHelper.Constants.AppointmentCompleted);

        await channel.DeclareQueueAsync(InfrastructureHelper.Constants.AppointmentUpdated);

        await Task.CompletedTask;
    }
}
