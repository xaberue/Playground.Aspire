using Grpc.Net.Client;
using Xaberue.Playground.HospitalManager.Patients;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;
using PatientsGrpc = Xaberue.Playground.HospitalManager.Patients.Patients;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class PatientGrpcService : IPatientService
{

    private readonly string _patientsApiUrl;


    public PatientGrpcService(String patientsApiUrl)
    {
        _patientsApiUrl = patientsApiUrl;
    }


    public async Task<IEnumerable<PatientGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        using var patientsChannel = GrpcChannel.ForAddress(_patientsApiUrl);

        var patientsClient = new PatientsGrpc.PatientsClient(patientsChannel);
        var response = await patientsClient.GetAllAsync(new GetAllPatientsRequest());

        return response.Patients.Select(x => new PatientGridViewModel(
                x.Id,
                x.Code,
                $"{x.Name} {x.Surname}",
                DateTime.Parse(x.DateOfBirth)
            ));
    }

    public async Task<IEnumerable<PatientSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        using var patientsChannel = GrpcChannel.ForAddress(_patientsApiUrl);

        var patientsClient = new PatientsGrpc.PatientsClient(patientsChannel);
        var response = await patientsClient.GetAllAsync(new GetAllPatientsRequest());

        return response.Patients.Select(x => new PatientSelectionViewModel(
                x.Id,
                x.Code,
                $"{x.Name} {x.Surname}"
            ));
    }
}
