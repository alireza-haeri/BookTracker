using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookTracker.EntityModels;

public class Book
{
    [NotMapped] [BsonIgnore] public const string CollectionName = "Books";

    public ObjectId Id { get; set; }
    public ObjectId UserId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }

    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookStatus Status { get; set; }

    public static Book Create(string title,string author,string notes,BookStatus status,ObjectId userId)
        => new()
        {
            Title = title,
            Author = author,
            Notes = notes,
            CreatedAt = DateTime.Now,
            UpdateAt = DateTime.Now,
            UserId = userId,
            Status = status
        };
}

public enum BookStatus
{
    /// <summary>
    /// The user is currently reading this book.
    /// </summary>
    Reading = 0,

    /// <summary>
    /// The user has finished reading this book.
    /// </summary>
    Completed = 1,

    /// <summary>
    /// The user wants to read this book in the future.
    /// </summary>
    Wishlist = 2,

    /// <summary>
    /// The user has paused reading this book temporarily.
    /// </summary>
    OnHold = 3,

    /// <summary>
    /// The user started reading but decided to abandon the book.
    /// </summary>
    Abandoned = 4,

    /// <summary>
    /// The user is reading this book again.
    /// </summary>
    ReReading = 5
}