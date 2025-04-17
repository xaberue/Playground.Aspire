using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Services;


[Authorize]
public class DoctorsGrpcService : Doctors.DoctorsBase
{

    private readonly DoctorsDbContext _dbContext;


    public DoctorsGrpcService(DoctorsDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public override Task<GetAllDoctorsResponse> GetAll(GetAllDoctorsRequest request, ServerCallContext context)
    {
        var doctors = _dbContext.Doctors.Select(x => x.ToGrpcModel()).ToList();
        var response = new GetAllDoctorsResponse();

        response.Doctors.AddRange(doctors);

        return Task.FromResult(response);
    }

    public override async Task<GetDoctorByIdResponse> GetDoctorById(GetDoctorByIdRequest request, ServerCallContext context)
    {
        var doctorEntity = await _dbContext.Doctors.FindAsync(request.Id);

        if (doctorEntity is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Doctor not found"));

        var response = new GetDoctorByIdResponse();
        response.Doctor = doctorEntity.ToGrpcModel();

        return response;
    }
}
