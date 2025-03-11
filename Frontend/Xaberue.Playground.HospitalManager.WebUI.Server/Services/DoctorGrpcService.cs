using Grpc.Net.Client;
using Xaberue.Playground.HospitalManager.Doctors;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;
using DoctorsGrpc = Xaberue.Playground.HospitalManager.Doctors.Doctors;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class DoctorGrpcService : IDoctorService
{

    private readonly string _doctorsApiUrl;


    public DoctorGrpcService(String doctorsApiUrl)
    {
        _doctorsApiUrl = doctorsApiUrl;
    }


    public async Task<IEnumerable<DoctorGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        using var doctorsChannel = GrpcChannel.ForAddress(_doctorsApiUrl);

        var doctorsClient = new DoctorsGrpc.DoctorsClient(doctorsChannel);
        var response = await doctorsClient.GetAllAsync(new GetAllDoctorsRequest());

        return response.Doctors.Select(x => new DoctorGridViewModel(
                x.Id,
                $"{x.Name} {x.Surname}",
                DateTime.Parse(x.HiringDate)
            ));
    }

    public async Task<IEnumerable<DoctorSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        using var doctorsChannel = GrpcChannel.ForAddress(_doctorsApiUrl);

        var doctorsClient = new DoctorsGrpc.DoctorsClient(doctorsChannel);
        var response = await doctorsClient.GetAllAsync(new GetAllDoctorsRequest());

        return response.Doctors.Select(x => new DoctorSelectionViewModel(
                x.Id,
                $"{x.Name} {x.Surname}"
            ));
    }
}

//TODO: Extract mappers