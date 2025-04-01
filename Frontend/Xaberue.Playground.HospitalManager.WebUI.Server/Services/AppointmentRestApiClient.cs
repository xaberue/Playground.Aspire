using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Doctors.Shared;
using Xaberue.Playground.HospitalManager.Patients.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Configuration;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class AppointmentRestApiClient : IAppointmentQueryApiService
{

    private readonly IHttpClientFactory _httpClientFactory;


    public AppointmentRestApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public async Task<IEnumerable<AppointmentGridViewModel>> GetAllToday(CancellationToken cancellationToken = default)
    {
        var appointmentsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.AppointmentsApiClient);
        var doctorsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.DoctorsApiClient);
        var patientsClient = _httpClientFactory.CreateClient(HospitalManagerApiConstants.PatientsApiClient);

        var appointmentsReponse = await appointmentsClient.GetFromJsonAsync<AppointmentsDetailsDto>("/appointments/today", cancellationToken: cancellationToken);
        var appointments = appointmentsReponse?.Appointments ?? [];
        var doctors = await doctorsClient.GetFromJsonAsync<DoctorDto[]>("/doctors", cancellationToken: cancellationToken) ?? [];
        var patients = await patientsClient.GetFromJsonAsync<PatientDto[]>($"/patients?ids={string.Join(',', appointments.Select(x => x.PatientId))}", cancellationToken: cancellationToken) ?? [];

        return appointments.Select(x =>
        {
            var doctor = doctors.First(d => d.Id == x.DoctorId);
            var patient = patients.First(p => p.Id == x.PatientId);

            return new AppointmentGridViewModel(
                x.Id,
                x.Code,
                x.DoctorId,
                $"{doctor.Name} {doctor.Surname}",
                x.PatientId,
                $"{patient.Name} {patient.Surname}",
                DateTime.Parse(x.Date),
                x.Box,
                x.Reason,
                x.CriticalityLevel,
                x.Status
            );
        });
    }
}
