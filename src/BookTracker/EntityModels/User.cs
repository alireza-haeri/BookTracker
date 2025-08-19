using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookTracker.EntityModels;

public class User
{
    [NotMapped][BsonIgnore] public const string CollectionName = "Users";

    private readonly List<Book> _books = [];
    
    public ObjectId Id { get; private init; }
    public string PhoneNumber { get; private set; }
    public string Password { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdateAt { get; private set; }

    public IReadOnlyList<Book> Books => _books.AsReadOnly();

    public static User Create(string phoneNumber, string password)
        => new()
        {
            Id = ObjectId.GenerateNewId(),
            PhoneNumber = phoneNumber,
            Password = password,
            CreatedAt = DateTime.Now,
            UpdateAt = DateTime.Now
        };
}