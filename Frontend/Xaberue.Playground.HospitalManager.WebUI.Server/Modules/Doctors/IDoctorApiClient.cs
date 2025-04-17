using Xaberue.Playground.HospitalManager.Doctors.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Doctors;

public interface IDoctorApiClient : IDoctorQueryApiService
{
    Task<DoctorDto?> GetAsync(int id, CancellationToken cancellationToken = default);
}
