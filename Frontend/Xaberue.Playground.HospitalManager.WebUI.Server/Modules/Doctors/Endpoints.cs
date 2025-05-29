using Microsoft.Extensions.Caching.Hybrid;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Doctors;

public static class Endpoints
{
    public static RouteGroupBuilder MapDoctorsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/doctors/grid", async (HybridCache cache, IDoctorQueryApiService doctorService, CancellationToken cancellationToken) =>
        {
            var data = await cache.GetOrCreateAsync("DOCTORS-GRID", async entry =>
            {
                return await doctorService.GetAllGridModelsAsync(entry);
            },
            tags: ["DOCTORS-GRID"],
            cancellationToken: cancellationToken
            );

            return data;
        })
        .WithName("GetAllDoctorGridModels");

        group.MapGet("/doctors/selection", async (IDoctorQueryApiService doctorService) =>
        {
            var data = await doctorService.GetAllSelectionModelsAsync();

            return data;
        })
        .WithName("GetAllDoctorSelectionModels");

        return group;
    }
}
