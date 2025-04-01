using Grpc.Net.Client;
using Xaberue.Playground.HospitalManager.Appointments;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Doctors;
using Xaberue.Playground.HospitalManager.Patients;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Contracts;
using Xaberue.Playground.HospitalManager.WebUI.Shared.Models;
using AppointmentsGrpc = Xaberue.Playground.HospitalManager.Appointments.Appointments;
using DoctorsGrpc = Xaberue.Playground.HospitalManager.Doctors.Doctors;
using PatientsGrpc = Xaberue.Playground.HospitalManager.Patients.Patients;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Services;

public class AppointmentGrpcApiClient : IAppointmentQueryApiService
{

    private readonly string _appointmentsApiUrl;
    private readonly string _doctorsApiUrl;
    private readonly string _patientsApiUrl;


    public AppointmentGrpcApiClient(string appointmentsApiUrl, string doctorsApiUrl, string patientsApiUrl)
    {
        _appointmentsApiUrl = appointmentsApiUrl;
        _doctorsApiUrl = doctorsApiUrl;
        _patientsApiUrl = patientsApiUrl;
    }


    public async Task<IEnumerable<AppointmentGridViewModel>> GetAllToday(CancellationToken cancellationToken = default)
    {
        using var appointmentsChannel = GrpcChannel.ForAddress(_appointmentsApiUrl);
        using var doctorsChannel = GrpcChannel.ForAddress(_doctorsApiUrl);
        using var patientsChannel = GrpcChannel.ForAddress(_patientsApiUrl);

        var appointmentsClient = new AppointmentsGrpc.AppointmentsClient(appointmentsChannel);
        var doctorsClient = new DoctorsGrpc.DoctorsClient(doctorsChannel);
        var patientsClient = new PatientsGrpc.PatientsClient(patientsChannel);

        var appointmentsResponse = await appointmentsClient.GetAllTodayAsync(new GetAllTodayAppointmentsRequest());
        var doctorResponse = await doctorsClient.GetAllAsync(new GetAllDoctorsRequest());
        var patientsRequest = new GetFilteredPatientsRequest();
        patientsRequest.Ids.AddRange([.. appointmentsResponse.Appointments.Select(x => x.PatientId).Distinct()]);
        var patientsResponse = await patientsClient.GetFilteredAsync(patientsRequest);

        return appointmentsResponse.Appointments.Select(x =>
        {
            var doctor = doctorResponse.Doctors.First(d => d.Id == x.DoctorId);
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
