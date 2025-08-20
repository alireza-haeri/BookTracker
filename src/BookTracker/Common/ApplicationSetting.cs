using MongoDB.Driver;

namespace BookTracker.Common;

public class ApplicationSetting
{
    public required JwtConfiguration JwtConfiguration { get; init; }
    public required MongoDbConfiguration MongoDbConfiguration { get; init; }
}

public class MongoDbConfiguration
{
    public required string ConnectionString { get; init; }
}

public class JwtConfiguration
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int ExpireMinute { get; init; }
}