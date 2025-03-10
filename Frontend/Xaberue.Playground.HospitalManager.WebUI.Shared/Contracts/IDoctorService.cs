using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

public interface IDoctorService
{
    Task<IEnumerable<DoctorGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default);
}
