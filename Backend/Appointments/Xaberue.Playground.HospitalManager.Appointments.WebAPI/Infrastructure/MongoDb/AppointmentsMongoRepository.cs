using MongoDB.Bson;
using MongoDB.Driver;
using Xaberue.Playground.HospitalManager.Appointments.Shared;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure.MongoDb;

public class AppointmentsMongoRepository : IAppointmentsRepository
{

    private IMongoCollection<AppointmentMongoDbModel> _collection;


    public AppointmentsMongoRepository(IMongoClient mongoDbClient)
    {
        var db = mongoDbClient.GetDatabase("AppointmentsDb");
        _collection = db.GetCollection<AppointmentMongoDbModel>("Appointments");
    }


    public async Task<IEnumerable<Appointment>> GetAllForToday()
    {
        var filter = Builders<AppointmentMongoDbModel>.Filter.Gt(a => a.Date, DateTime.Today);
        var models = await _collection.Find(filter).ToListAsync();

        return models.Select(x => x.ToEntity());
    }

    public async Task<IEnumerable<Appointment>> GetAllCurrentActive()
    {
        var filter = Builders<AppointmentMongoDbModel>.Filter.Gt(a => a.Date, DateTime.Today) &
            Builders<AppointmentMongoDbModel>.Filter.Ne(a => a.Status, AppointmentStatus.Completed);
        var models = await _collection.Find(filter).ToListAsync();

        return models.Select(x => x.ToEntity());
    }

    public async Task InsertAsync(Appointment appointment)
    {
        var dbModel = appointment.ToDbModel();

        await _collection.InsertOneAsync(dbModel);

        appointment.Id = dbModel.Id.ToString();
    }

    public async Task<Appointment> UpdateAsync(string id, string box, AppointmentStatus status)
    {
        var appointment = await _collection.FindOneAndUpdateAsync<AppointmentMongoDbModel>(
                Builders<AppointmentMongoDbModel>.Filter.Eq(a => a.Id, ObjectId.Parse(id)),
                Builders<AppointmentMongoDbModel>.Update.Combine(
                    Builders<AppointmentMongoDbModel>.Update.Set(a => a.Status, status),
                    Builders<AppointmentMongoDbModel>.Update.Set(a => a.Box, box)
                ),
                new FindOneAndUpdateOptions<AppointmentMongoDbModel>
                {
                    ReturnDocument = ReturnDocument.After
                }
            );

        return appointment.ToEntity();
    }
}
