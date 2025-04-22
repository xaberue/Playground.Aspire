using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Doctors;
using Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Patients;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Modules.Appointments;

public class AppointmentGrpcApiService : IAppointmentQueryApiService
{

    private readonly AppointmentGrpcApiClient _appointmentGrpcApiClient;
    private readonly DoctorGrpcApiClient _doctorGrpcApiClient;
    private readonly PatientGrpcApiClient _patientGrpcApiClient;


    public AppointmentGrpcApiService(AppointmentGrpcApiClient appointmentGrpcApiClient, DoctorGrpcApiClient doctorGrpcApiClient, PatientGrpcApiClient patientGrpcApiClient)
    {
        _appointmentGrpcApiClient = appointmentGrpcApiClient;
        _doctorGrpcApiClient = doctorGrpcApiClient;
        _patientGrpcApiClient = patientGrpcApiClient;
    }


    public async Task<IEnumerable<AppointmentGridViewModel>> GetAllToday(CancellationToken cancellationToken = default)
    {
        var appointmentsResponse = await _appointmentGrpcApiClient.GetAllTodayAsync(cancellationToken);
        var doctorsResponse = await _doctorGrpcApiClient.GetAllAsync(cancellationToken);
        var patientsResponse = await _patientGrpcApiClient.GetFilteredAsync([.. appointmentsResponse.Appointments.Select(x => x.PatientId).Distinct()], cancellationToken);

        return appointmentsResponse.Appointments.Select(x =>
        {
            var doctor = doctorsResponse.Doctors.First(d => d.Id == x.DoctorId);
            var patient = patientsResponse.Patients.First(p => p.Id == x.PatientId);

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
                (x.Criticality >= 0) ? (CriticalityLevel)x.Criticality : null,
                (AppointmentStatus)x.Status
            );
        });
    }
}
