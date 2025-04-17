using Xaberue.Playground.HospitalManager.Doctors;
using Xaberue.Playground.HospitalManager.Doctors.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Base;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;
using DoctorsGrpc = Xaberue.Playground.HospitalManager.Doctors.Doctors;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Doctors;

public class DoctorGrpcApiService : IDoctorApiClient
{

    private readonly DoctorGrpcApiClient _doctorGrpcApiClient;


    public DoctorGrpcApiService(DoctorGrpcApiClient doctorGrpcApiClient)
    {
        _doctorGrpcApiClient = doctorGrpcApiClient;
    }


    public async Task<DoctorDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _doctorGrpcApiClient.GetAsync(id, cancellationToken);

        return new DoctorDto(
            response.Doctor.Id,
            response.Doctor.Name,
            response.Doctor.Surname,
            response.Doctor.BoxAssigned,
            DateTime.Parse(response.Doctor.HiringDate)
        );
    }

    public async Task<IEnumerable<DoctorGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _doctorGrpcApiClient.GetAllAsync(cancellationToken);

        return response.Doctors.Select(x => new DoctorGridViewModel(
                x.Id,
                $"{x.Name} {x.Surname}",
                DateTime.Parse(x.HiringDate)
            ));
    }

    public async Task<IEnumerable<DoctorSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _doctorGrpcApiClient.GetAllAsync(cancellationToken);

        return response.Doctors.Select(x => new DoctorSelectionViewModel(
                x.Id,
                $"{x.Name} {x.Surname}"
            ));
    }

}

//TODO: Extract mappers