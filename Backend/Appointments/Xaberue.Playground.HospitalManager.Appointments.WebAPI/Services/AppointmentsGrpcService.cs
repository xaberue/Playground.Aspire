using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;


[Authorize]
public class AppointmentsGrpcService : Appointments.AppointmentsBase
{

    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly AppointmentDailyCodeGeneratorService _appointmentCodeGeneratorService;


    public AppointmentsGrpcService(IAppointmentsRepository appointmentsRepository, AppointmentDailyCodeGeneratorService appointmentCodeGeneratorService)
    {
        _appointmentsRepository = appointmentsRepository;
        _appointmentCodeGeneratorService = appointmentCodeGeneratorService;
    }


    public override async Task<AppointmentDetailsCollection> GetAllToday(GetAllTodayAppointmentsRequest request, ServerCallContext context)
    {
        var appointments = (await _appointmentsRepository.GetAllForToday()).Select(x => x.ToDetailGrpcModel());
        var response = new AppointmentDetailsCollection();
        response.Appointments.AddRange(appointments);

        return response;
    }

    public override async Task<AppointmentSummariesCollection> GetAllCurrentActive(GetAllCurrentActiveAppointmentsRequest request, ServerCallContext context)
    {
        var appointments = (await _appointmentsRepository.GetAllCurrentActive()).Select(x => x.ToSummaryGrpcModel());
        var response = new AppointmentSummariesCollection();
        response.Appointments.AddRange(appointments);

        return response;
    }

    public override async Task<CreateAppointmentResponse> Create(CreateAppointmentRequest request, ServerCallContext context)
    {
        if (request.DoctorId <= 0 || request.PatientId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "DoctorId and/or PatientId must be set and valid"));

        var generatedCode = await _appointmentCodeGeneratorService.GenerateAsync();
        var appointment = new Appointment(request.PatientId, request.DoctorId, generatedCode, request.Reason);

        await _appointmentsRepository.InsertAsync(appointment);

        var response = new CreateAppointmentResponse { Appointment = appointment.ToSummaryGrpcModel() };

        return response;
    }

}
