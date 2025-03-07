namespace Xaberue.Playground.HospitalManager.Shared;

public record DoctorGridViewModel(int Id, string FullName, DateTime HiringDate);

public record DoctorSelectionViewModel(int Id, string FullName);