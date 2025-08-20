using BookTracker.Common;
using BookTracker.EntityModels;
using Carter;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookTracker.Endpoints.Book;

public class UpdateBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/book/{id}", async (
            string id,
            UpdateBookRequest request,
            IMongoDatabase db,
            HttpContext context,
            IValidator<UpdateBookRequest> validator) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            if (!ObjectId.TryParse(id, out var objectId))
                return Results.BadRequest("Invalid book ID format.");

            var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.UserId)?.Value;
            if (userIdClaim is null || !ObjectId.TryParse(userIdClaim, out var userId))
                return Results.BadRequest("UserId is missing or invalid.");

            var collection = db.GetCollection<EntityModels.Book>("Books");

            var filter = Builders<EntityModels.Book>.Filter.Where(x => x.Id == objectId && x.UserId == userId);
            var update = Builders<EntityModels.Book>.Update
                .Set(x => x.Title, request.Title)
                .Set(x => x.Author, request.Author)
                .Set(x => x.Status, request.Status)
                .Set(x => x.UpdateAt, DateTime.Now);

            var result = await collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return Results.NotFound($"No book found with ID {id} for the current user.");

            return Results.Ok($"Book with ID {id} has been successfully updated.");
        });
    }

    public record UpdateBookRequest(
        string Title,
        string Author,
        BookStatus Status
    );

    public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
    {
        public UpdateBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100);

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100);

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .IsInEnum().WithMessage("Status is invalid.");
        }
    }
}