using Grpc.Core;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Services;

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

}
