using MongoDB.Bson;
using MongoDB.Driver;

namespace Xaberue.Playground.HospitalManager.Appointments.WebAPI.Services;

public class AppointmentDailyCodeGeneratorService
{

    private readonly IMongoCollection<BsonDocument> _dailyCountersCollection;


    public AppointmentDailyCodeGeneratorService(IMongoDatabase database)
    {
        _dailyCountersCollection = database.GetCollection<BsonDocument>("AppointmentDailyCounters");
    }


    public async Task<string> GenerateAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

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

        return $"V-{counter}";
    }

}