using MongoDB.Bson;

namespace BookTracker.EntityModels;

public class User
{
    public ObjectId Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }

    public List<Book> Books { get; set; }
}