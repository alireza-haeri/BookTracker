using MongoDB.Driver;

namespace BookTracker.MongoDb.Configuration;

public interface IMongoDbEntityConfiguration
{
    Task ConfigureAsync(IMongoDatabase database);
}