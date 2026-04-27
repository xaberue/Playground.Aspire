using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;

public class AppointmentDailyCodeGeneratorService
{

    private readonly IAppointmentDailyCountersRepository _appointmentDailyCountersRepository;


    public AppointmentDailyCodeGeneratorService(IAppointmentDailyCountersRepository appointmentDailyCountersRepository)
    {
        _appointmentDailyCountersRepository = appointmentDailyCountersRepository;
    }


    public async Task<string> GenerateAsync()
    {
        var counter = await _appointmentDailyCountersRepository.GetForAsync(DateTime.UtcNow);

        return $"V-{counter}";
    }


}