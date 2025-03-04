using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Patients.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Patients.WebAPI.Services;

public class PatientsGrpcService : Patients.PatientsBase
{

    private readonly PatientsDbContext _dbContext;


    public PatientsGrpcService(PatientsDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public override async Task<GetPatientResponse> Get(GetPatientRequest request, ServerCallContext context)
    {
        var patient = await _dbContext.Patients.FirstAsync(x => x.Code == request.Code) ??
                    throw new RpcException(new Status(StatusCode.NotFound, $"Patient with code {request.Code} not found"));
        var response = new GetPatientResponse();

        response.Patient = patient.ToGrpcModel();

        return response;
    }

    public override Task<GetAllPatientsResponse> GetAll(GetAllPatientsRequest request, ServerCallContext context)
    {
        var patients = _dbContext.Patients.Select(x => x.ToGrpcModel()).ToList();
        var response = new GetAllPatientsResponse();

        response.Patients.AddRange(patients);

        return Task.FromResult(response);
    }
}
