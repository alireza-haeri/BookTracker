using BookTracker.EntityModels;
using BookTracker.MongoDb.Configuration;
using MongoDB.Driver;

namespace BookTracker.EntityConfiguration;

public class UserEntityConfiguration : IMongoDbEntityConfiguration
{
    public async Task ConfigureAsync(IMongoDatabase database)
    {
        var indexModel = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Text(i=>i.PhoneNumber));
        
        var collection = database.GetCollection<User>(User.CollectionName);
        await collection.Indexes.CreateOneAsync(indexModel);
    }
}