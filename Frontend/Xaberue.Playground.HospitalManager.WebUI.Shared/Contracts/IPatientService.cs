using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;

public interface IPatientService
{
    Task<IEnumerable<PatientGridViewModel>> GetAllGridModelsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientSelectionViewModel>> GetAllSelectionModelsAsync(CancellationToken cancellationToken = default);
}
