using Xaberue.Playground.HospitalManager.Patients;
using Xaberue.Playground.HospitalManager.WebUI.Server.Base;
using PatientsGrpc = Xaberue.Playground.HospitalManager.Patients.Patients;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Patients;

public class PatientGrpcApiClient : GrpcApiClientBase
{

    public PatientGrpcApiClient(string patientsApiUrl, string patientsApiKey)
        : base(patientsApiUrl, patientsApiKey)
    { }


    public async Task<GetPatientResponse> GetAsync(string code, CancellationToken cancellationToken = default)
    {
        var patientsClient = new PatientsGrpc.PatientsClient(GrpcChannel);
        var request = new GetPatientRequest { Code = code };

        var response = await patientsClient.GetAsync(request, headers: Headers, cancellationToken: cancellationToken);

        return response;
    }

    public async Task<GetPatientsResponse> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var patientsClient = new PatientsGrpc.PatientsClient(GrpcChannel);
        var response = await patientsClient.GetAllAsync(new GetAllPatientsRequest(), headers: Headers, cancellationToken: cancellationToken);

        return response;
    }

    public async Task<GetPatientsResponse> GetFilteredAsync(int[] ids, CancellationToken cancellationToken = default)
    {
        var patientsClient = new PatientsGrpc.PatientsClient(GrpcChannel);
        var patientsRequest = new GetFilteredPatientsRequest();
        patientsRequest.Ids.AddRange([.. ids]);
        var response = await patientsClient.GetFilteredAsync(patientsRequest, headers: Headers, cancellationToken: cancellationToken);

        return response;
    }

}
