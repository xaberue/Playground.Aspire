using MongoDB.Bson;
using MongoDB.Driver;
using Xaberue.Playground.HospitalManager.Appointments.WebAPI.Models;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Infrastructure;

public class AppointmentDailyCountersMongoRepository : IAppointmentDailyCountersRepository
{

    private readonly IMongoCollection<BsonDocument> _dailyCountersCollection;


    public AppointmentDailyCountersMongoRepository(IMongoDatabase database)
    {
        _dailyCountersCollection = database.GetCollection<BsonDocument>("AppointmentDailyCounters");
    }


    public async Task<int> GetForAsync(DateTime date)
    {
        var today = date.ToString("yyyy-MM-dd");
        var update = Builders<BsonDocument>.Update.Inc("counter", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var counterDoc = await _dailyCountersCollection.FindOneAndUpdateAsync(
            Builders<BsonDocument>.Filter.Eq("_id", today),
            update,
            options
        );

        var counter = counterDoc["counter"].ToInt32();
        return counter;
    }
}
