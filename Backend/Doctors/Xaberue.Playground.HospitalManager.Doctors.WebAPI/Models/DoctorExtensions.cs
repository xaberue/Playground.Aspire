namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

public static class DoctorExtensions
{
    public static DoctorDto ToDto(this Doctor entity)
       => new DoctorDto
       {
           Id = entity.Id,
           Name = entity.Name
       };

}
