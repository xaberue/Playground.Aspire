using Xaberue.Playground.HospitalManager.WebUI.Server.Hubs;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Appointments;

public static class Endpoints
{
    public static RouteGroupBuilder MapAppointmentsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/appointments/today", async (IAppointmentQueryApiService appointmentService, ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("GetAllTodayAppointments");
            logger.LogInformation("GetAllTodayAppointments called");

            var data = await appointmentService.GetAllToday();

            return data;
        })
        .WithName("GetAllTodayAppointments");

        group.MapPost("/appointment", async (IAppointmentCommandApiService appointmentService, AppointmentRegistrationViewModel creationModel, CancellationToken cancellationToken) =>
        {
            await appointmentService.RegisterAsync(creationModel, cancellationToken);

            return Results.Accepted();
        })
        .WithName("RegisterAppointment");

        group.MapPut("/appointment/admit", async (IAppointmentCommandApiService appointmentService, AppointmentAdmissionViewModel admissionModel, CancellationToken cancellationToken) =>
        {
            await appointmentService.AdmitAsync(admissionModel, cancellationToken);

            return Results.Accepted();
        })
        .WithName("AdmitAppointment");

        group.MapPut("/appointment/complete", async (IAppointmentCommandApiService appointmentService, AppointmentCompletionViewModel completionModel, CancellationToken cancellationToken) =>
        {
            await appointmentService.CompleteAsync(completionModel, cancellationToken);

            return Results.Accepted();
        })
        .WithName("CompleteAppointment");

        return group;
    }

    public static WebApplication MapAppointmentsHubs(this WebApplication app)
    {
        app.MapHub<AppointmentHub>("hub/appointment-updated");

        return app;
    }
}
