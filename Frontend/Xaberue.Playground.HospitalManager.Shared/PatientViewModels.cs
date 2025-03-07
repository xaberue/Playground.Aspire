namespace Xaberue.Playground.HospitalManager.Shared;

public record PatientGridViewModel(int Id, string Code, string FullName, DateTime DateOfBirth);

public record PatientSelectionViewModel(int Id, string Code, string FullName);
