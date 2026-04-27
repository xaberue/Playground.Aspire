using MongoDB.Bson;
using MongoDB.Driver;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;

public class AppointmentsMongoRepository : IAppointmentsRepository
{

    private IMongoCollection<Appointment> _collection;


    public AppointmentsMongoRepository(IMongoClient mongoDbClient)
    {
        var db = mongoDbClient.GetDatabase("AppointmentsDb");
        _collection = db.GetCollection<Appointment>("Appointments");
    }


    public async Task<IEnumerable<Appointment>> GetAllForToday()
    {
        var filter = Builders<Appointment>.Filter.Gt(a => a.Date, DateTime.Today);

        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAllCurrentActive()
    {
        var filter = Builders<Appointment>.Filter.Gt(a => a.Date, DateTime.Today) &
            Builders<Appointment>.Filter.Ne(a => a.Status, AppointmentStatus.Completed);

        return await _collection.Find(filter).ToListAsync();
    }

    public async Task InsertAsync(Appointment appointment)
    {
        await _collection.InsertOneAsync(appointment);
    }

    public async Task<Appointment> UpdateAsync(string id, string box, AppointmentStatus status)
    {
        var appointment = await _collection.FindOneAndUpdateAsync<Appointment>(
                Builders<Appointment>.Filter.Eq(a => a.Id, ObjectId.Parse(id)),
                Builders<Appointment>.Update.Combine(
                    Builders<Appointment>.Update.Set(a => a.Status, status),
                    Builders<Appointment>.Update.Set(a => a.Box, box)
                ),
                new FindOneAndUpdateOptions<Appointment>
                {
                    ReturnDocument = ReturnDocument.After
                }
            );

        return appointment;
    }
}
