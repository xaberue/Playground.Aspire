using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Patients.WebAPI.Services;

public class PatientsGrpcService : Patients.PatientsBase
{

    private readonly PatientsDbContext _patientsDbContext;


    public PatientsGrpcService(PatientsDbContext patientsDbContext)
    {
        _patientsDbContext = patientsDbContext;
    }


    public override async Task<GetPatientResponse> Get(GetPatientRequest request, ServerCallContext context)
    {
        var patient = await _patientsDbContext.Patients.FirstAsync(x => x.Code == request.Code) ??
                    throw new RpcException(new Status(StatusCode.NotFound, $"Patient with code {request.Code} not found"));
        var response = new GetPatientResponse();

        response.Patient = patient.ToGrpcModel();

        return response;
    }

}
