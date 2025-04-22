namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

public record DoctorGridViewModel(int Id, string FullName, DateTime HiringDate);

public record DoctorSelectionViewModel(int Id, string FullName);