using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Patients;

public static class Endpoints
{

    public static RouteGroupBuilder MapPatientsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/patients/grid", async (IPatientQueryApiService patientService) =>
        {
            var data = await patientService.GetAllGridModelsAsync();

            return data;
        })
        .WithName("GetAllPatientGridModels");

        group.MapGet("/patients/{code}", async (string code, IPatientQueryApiService patientService) =>
        {
            var data = await patientService.GetSelectionModelAsync(code);

            return data;
        })
        .WithName("GetPatientSelectionModelByCode");

        group.MapGet("/patients/selection", async (IPatientQueryApiService patientService) =>
        {
            var data = await patientService.GetAllSelectionModelsAsync();

            return data;
        })
        .WithName("GetAllPatientSelectionModels");

        return group;
    }

}
