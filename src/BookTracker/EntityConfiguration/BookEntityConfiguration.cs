using BookTracker.EntityModels;
using BookTracker.MongoDb.Configuration;
using MongoDB.Driver;

namespace BookTracker.EntityConfiguration;

public class BookEntityConfiguration : IMongoDbEntityConfiguration
{
    public async Task ConfigureAsync(IMongoDatabase database)
    {
        var textIndex = new CreateIndexModel<Book>(
            Builders<Book>.IndexKeys
                .Text(b => b.Title)
                .Text(b => b.UserId)
        );

        var collection = database.GetCollection<Book>(Book.CollectionName);
        await collection.Indexes.CreateOneAsync(textIndex);
    }
}