using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xaberue.Playground.HospitalManager.Doctors.Shared;

namespace Xaberue.Playground.HospitalManager.Doctors.WebAPI.Models;

public static class DoctorExtensions
{
    public static DoctorDto ToDto(this Doctor entity)
        => new(entity.Id, entity.Name, entity.Surname, entity.BoxAssigned, entity.HiringDate);

    public static DoctorModel ToGrpcModel(this Doctor entity)
       => new DoctorModel
       {
           Id = entity.Id,
           Name = entity.Name,
           Surname = entity.Surname,
           BoxAssigned = entity.BoxAssigned,
           HiringDate = entity.HiringDate.ToString()
       };

}
