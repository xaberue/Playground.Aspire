using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class AppointmentRabbitService : IAppointmentService
{
    public Task CreateAsync(AppointmentCreationViewModel creationViewModel, CancellationToken cancellationToken = default)
    {
        //TODO: Implement

        return Task.CompletedTask;
    }
}
