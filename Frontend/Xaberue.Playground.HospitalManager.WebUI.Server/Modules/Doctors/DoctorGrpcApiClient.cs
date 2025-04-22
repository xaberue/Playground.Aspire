using Xaberue.Playground.HospitalManager.Doctors;
using Xaberue.Playground.HospitalManager.Doctors.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Base;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;
using DoctorsGrpc = Xaberue.Playground.HospitalManager.Doctors.Doctors;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Doctors;

public class DoctorGrpcApiClient : GrpcApiClientBase
{

    public DoctorGrpcApiClient(string doctorsApiUrl, string doctorsApiKey)
        : base(doctorsApiUrl, doctorsApiKey)
    { }


    public async Task<GetDoctorByIdResponse> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var doctorsClient = new DoctorsGrpc.DoctorsClient(GrpcChannel);
        var response = await doctorsClient.GetDoctorByIdAsync(new GetDoctorByIdRequest { Id = id }, headers: Headers, cancellationToken: cancellationToken);

        return response;
    }

    public async Task<GetAllDoctorsResponse> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var doctorsClient = new DoctorsGrpc.DoctorsClient(GrpcChannel);
        var response = await doctorsClient.GetAllAsync(new GetAllDoctorsRequest(), headers: Headers, cancellationToken: cancellationToken);

        return response;
    }

}
