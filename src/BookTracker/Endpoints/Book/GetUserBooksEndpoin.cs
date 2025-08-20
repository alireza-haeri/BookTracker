using BookTracker.Common;
using Carter;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookTracker.Endpoints.Book;

public class GetUserBooksEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/user/books", async ( IMongoDatabase db,HttpContext context) =>
        {
            var userIdClaim = context.User.Claims.FirstOrDefault(c=>c.Type == JwtClaimTypes.UserId)?.Value;
            if (userIdClaim is null)
                return Results.BadRequest("UserId not found");

            if (!ObjectId.TryParse(userIdClaim, out var userId))
                return Results.BadRequest("UserId not valid");
            
            var userCollection = db.GetCollection<EntityModels.User>(EntityModels.User.CollectionName);
            var bookCollection = db.GetCollection<EntityModels.Book>(EntityModels.Book.CollectionName);

            var userFilter = Builders<EntityModels.User>.Filter.Eq(u => u.Id, userId);
            var user = await userCollection.Find(userFilter).FirstOrDefaultAsync();

            if (user is null)
                return Results.NotFound($"User not found");

            var bookFilter = Builders<EntityModels.Book>.Filter.Eq(b => b.UserId, user.Id);
            var books = await bookCollection.Find(bookFilter).ToListAsync();

            return Results.Ok(books.Select(b => new BookResponse(
                b.Id.ToString(),
                b.Title,
                b.Author,
                b.CreatedAt
            )));
        })
        .RequireAuthorization()
        .WithTags("Book");
    }

    public record BookResponse(
        string Id,
        string Title,
        string Author,
        DateTime CreatedAt
    );
}