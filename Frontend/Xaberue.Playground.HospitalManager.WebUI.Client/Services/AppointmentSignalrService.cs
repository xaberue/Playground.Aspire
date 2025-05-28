using Microsoft.AspNetCore.SignalR.Client;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Client.Services;

public class AppointmentSignalrService : IAsyncDisposable
{

    private readonly HubConnection _hubConnection;


    public AppointmentSignalrService(string baseUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}hub/appointment-updated")
            .Build();
    }

    public async Task Configure(Action<AppointmentUpdatedViewModel> handler)
    {
        _hubConnection.On<AppointmentUpdatedViewModel>("ReceiveAppointmentUpdated", handler);
        await _hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.StopAsync();
    }
}
