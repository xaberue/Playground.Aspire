namespace Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

public record PatientGridViewModel(int Id, string Code, string FullName, DateTime DateOfBirth);

public record PatientSelectionViewModel(int Id, string Code, string FullName);
