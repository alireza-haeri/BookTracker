using BookTracker.MongoDb.Configuration;
using MongoDB.Driver;

namespace BookTracker.MongoDb.Extensions;

public static class MongoDbConfigurationExtensions
{
    public static WebApplicationBuilder ConfigureMongoDb(this WebApplicationBuilder builder)
    {
        var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));

        var mongoDatabase = mongoClient.GetDatabase("BookTracker");

        builder.Services.AddSingleton(mongoDatabase);

        return builder;
    }

    public static WebApplicationBuilder ConfigureMongoDbEntities(this WebApplicationBuilder builder)
    {
        var configurationTypeServices=
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IMongoDbEntityConfiguration).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
                .Select(c=>new ServiceDescriptor(typeof(IMongoDbEntityConfiguration),c,ServiceLifetime.Singleton));
        
        foreach (var configurationTypeService in configurationTypeServices)
            builder.Services.Add(configurationTypeService);
        
        return builder;
    }

    public static async Task UseMongoDbEntitiesAsync(this WebApplication app)
    {
        var mongoDb = app.Services.GetRequiredService<IMongoDatabase>();
        var configurations = app.Services.GetServices<IMongoDbEntityConfiguration>();

        foreach (var configuration in configurations)
            await configuration.ConfigureAsync(mongoDb);
    }
}