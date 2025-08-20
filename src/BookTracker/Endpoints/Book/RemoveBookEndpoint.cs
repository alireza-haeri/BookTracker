using BookTracker.Common;
using Carter;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookTracker.Endpoints.Book;

public class RemoveBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/book/{id}", async (string id, IMongoDatabase db, HttpContext context) =>
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return Results.BadRequest("Invalid book ID format.");
            }

            var userIdClaim = context.User.Claims.FirstOrDefault(c=>c.Type == JwtClaimTypes.UserId)?.Value;
            if (userIdClaim is null)
                return Results.BadRequest("UserId not found");

            if (!ObjectId.TryParse(userIdClaim, out var userId))
                return Results.BadRequest("UserId not valid");
            
            var collection = db.GetCollection<EntityModels.Book>(EntityModels.Book.CollectionName);

            var result = await collection.DeleteOneAsync(x => x.Id == objectId && x.UserId == userId);

            if (result.DeletedCount == 0)
                return Results.NotFound($"No book found with ID {id} for the current user.");

            return Results.Ok($"Book with ID {id} has been successfully removed.");
        }).RequireAuthorization()
        .WithTags("Book");
    }
}