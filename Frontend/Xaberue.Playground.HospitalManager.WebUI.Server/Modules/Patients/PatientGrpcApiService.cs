using Grpc.Core;
using Grpc.Net.Client;
using RabbitMQ.Client;
using Xaberue.Playground.HospitalManager.Patients;
using Xaberue.Playground.HospitalManager.WebUI.Server.Base;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;
using PatientsGrpc = Xaberue.Playground.HospitalManager.Patients.Patients;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Patients;

public class PatientGrpcApiService : IPatientQueryApiService
{

    private readonly PatientGrpcApiClient _patientGrpcApiClient;


    public PatientGrpcApiService(PatientGrpcApiClient patientGrpcApiClient) 
    {
        _patientGrpcApiClient = patientGrpcApiClient;
    }


    public async Task<IEnumerable<PatientGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _patientGrpcApiClient.GetAllAsync(cancellationToken);

        return response.Patients.Select(x => new PatientGridViewModel(
                x.Id,
                x.Code,
                $"{x.Name} {x.Surname}",
                DateTime.Parse(x.DateOfBirth)
            ));
    }

    public async Task<PatientSelectionViewModel?> GetSelectionModelAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _patientGrpcApiClient.GetAsync(code, cancellationToken);

            return new PatientSelectionViewModel(
                    response.Patient.Id,
                    response.Patient.Code,
                    $"{response.Patient.Name} {response.Patient.Surname}"
                );
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<PatientSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _patientGrpcApiClient.GetAllAsync(cancellationToken);

        return response.Patients.Select(x => new PatientSelectionViewModel(
                x.Id,
                x.Code,
                $"{x.Name} {x.Surname}"
            ));
    }

}

//TODO: Extract mappers