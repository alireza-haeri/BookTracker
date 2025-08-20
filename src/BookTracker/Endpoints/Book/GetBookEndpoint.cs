using System.Text.Json.Serialization;
using BookTracker.Common;
using Carter;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookTracker.Endpoints.Book;

public class GetBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/book/{id}", async ( string id,IMongoDatabase db,HttpContext context) =>
        {
            var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.UserId)?.Value;
            if (userIdClaim is null)
                return Results.BadRequest("UserId not found");

            if (!ObjectId.TryParse(userIdClaim, out var userId))
                return Results.BadRequest("UserId not valid");
            
            if(!ObjectId.TryParse(id, out var bookId))
                return Results.Problem("BookId not valid",statusCode: StatusCodes.Status400BadRequest);
            
            var bookCollection = db.GetCollection<EntityModels.Book>(EntityModels.Book.CollectionName);
            
            var filter = Builders<EntityModels.Book>.Filter.Where(b=>b.Id == bookId && b.UserId == userId);
            var result = await bookCollection.Find(filter).FirstOrDefaultAsync();
            
            if(result is null)
                return Results.NotFound("Book not found");

            return Results.Ok(new GetBookResult(result.Id.ToString(), result.Title, result.Author,
                result.Status.ToString(), result.Notes));
        })
        .RequireAuthorization()
        .WithTags("Book");
    }
    
    public record GetBookResult(string BookId,string Title,string Author,string Status,string Notes);
}