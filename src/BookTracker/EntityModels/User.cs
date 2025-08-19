using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookTracker.EntityModels;

public class User
{
    [NotMapped][BsonIgnore] public const string CollectionName = "Users";
    
    public ObjectId Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }

    public List<Book> Books { get; set; }
}